// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Filters;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionInvoker : IActionInvoker
    {
        private readonly IReadOnlyList<IFilterProvider> _filterProviders;
        private readonly IModuleFactory _moduleFactory;

        private ModuleExecutingContext _moduleExecutingContext;
        private ModuleExecutedContext _moduleExecutedContext;
        private ResultExecutingContext _resultExecutingContext;
        private ResultExecutedContext _resultExecutedContext;

        private FilterCursor _cursor;
        private IFilterMetadata[] _filters;
        private MvcModule _module;

        public ModuleActionInvoker(
            IReadOnlyList<IFilterProvider> filterProviders,
            IModuleFactory moduleFactory,
            ActionContext actionContext)
        {
            _filterProviders = filterProviders;
            _moduleFactory = moduleFactory;

            Context = new ModuleContext(actionContext);
        }

        protected ModuleContext Context { get; }

        public async Task InvokeAsync()
        {
            _filters = GetFilters();
            _cursor = new FilterCursor(_filters);

            _module = (MvcModule)_moduleFactory.CreateModule(Context);
            using (_module as IDisposable)
            {
                await InvokeAllModuleFiltersAsync();
                if (_moduleExecutedContext.Canceled)
                {
                    await _moduleExecutedContext.Result.ExecuteResultAsync(Context);
                    return;
                }

                await InvokeAllResultFiltersAsync(_moduleExecutedContext.Result);
            }
        }

        private IActionResult CreateActionResult(object value)
        {
            // optimize common path
            var actionResult = value as IActionResult;
            if (actionResult != null)
            {
                return actionResult;
            }
            else if (value == null)
            {
                return new NoContentResult();
            }
            else
            {
                return new ObjectResult(value);
            }
        }

        private Task InvokeAllModuleFiltersAsync()
        {
            _cursor.Reset();
            _module = (MvcModule)_moduleFactory.CreateModule(Context);
            _moduleExecutingContext = new ModuleExecutingContext(Context, _filters, _module);

            return InvokeModuleFilterAsync();
        }

        private async Task<ModuleExecutedContext> InvokeModuleFilterAsync()
        {
            Debug.Assert(_moduleExecutingContext != null);
            if (_moduleExecutingContext.Result != null)
            {
                throw new InvalidOperationException();
            }

            var item = _cursor.GetNextFilter<IModuleFilter, IAsyncModuleFilter>();
            try
            {
                if (item.FilterAsync != null)
                {
                    await item.FilterAsync.OnModuleExecutionAsync(_moduleExecutingContext, InvokeModuleFilterAsync);

                    if (_moduleExecutedContext == null)
                    {
                        _moduleExecutedContext = new ModuleExecutedContext(Context, _filters, _module)
                        {
                            Canceled = true,
                            Result = _moduleExecutingContext.Result,
                        };
                    }
                }
                else if (item.Filter != null)
                {
                    item.Filter.OnModuleExecuting(_moduleExecutingContext);

                    if (_moduleExecutingContext.Result != null)
                    {
                        // Short-circuited by setting a result.
                        _moduleExecutedContext = new ModuleExecutedContext(_moduleExecutingContext, _filters, _module)
                        {
                            Canceled = true,
                            Result = _moduleExecutingContext.Result,
                        };
                    }
                    else
                    {
                        item.Filter.OnModuleExecuted(await InvokeModuleFilterAsync());
                    }
                }
                else
                {
                    // All action filters have run, execute the action method.
                    var actionInfo = _module.Actions[Context.ActionDescriptor.Index];
                    var value = actionInfo.Func();
                    var result = CreateActionResult(value);

                    _moduleExecutedContext = new ModuleExecutedContext(_moduleExecutingContext, _filters, _module)
                    {
                        Result = result
                    };
                }
            }
            catch (Exception exception)
            {
                // Exceptions thrown by the action method OR filters bubble back up through ActionExcecutedContext.
                _moduleExecutedContext = new ModuleExecutedContext(_moduleExecutingContext, _filters, _module)
                {
                    ExceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception)
                };
            }
            return _moduleExecutedContext;
        }

        private async Task InvokeAllResultFiltersAsync(IActionResult result)
        {
            _cursor.Reset();

            _resultExecutingContext = new ResultExecutingContext(Context, _filters, result, _module);
            await InvokeResultFilterAsync();

            Debug.Assert(_resultExecutingContext != null);
            if (_resultExecutedContext.Exception != null && !_resultExecutedContext.ExceptionHandled)
            {
                // There's an unhandled exception in filters
                if (_resultExecutedContext.ExceptionDispatchInfo != null)
                {
                    _resultExecutedContext.ExceptionDispatchInfo.Throw();
                }
                else
                {
                    throw _resultExecutedContext.Exception;
                }
            }
        }

        private async Task<ResultExecutedContext> InvokeResultFilterAsync()
        {
            Debug.Assert(_resultExecutingContext != null);
            if (_resultExecutingContext.Cancel == true)
            {
                throw new InvalidOperationException();
            }

            try
            {
                var item = _cursor.GetNextFilter<IResultFilter, IAsyncResultFilter>();
                if (item.FilterAsync != null)
                {
                    await item.FilterAsync.OnResultExecutionAsync(_resultExecutingContext, InvokeResultFilterAsync);

                    if (_resultExecutedContext == null || _resultExecutingContext.Cancel == true)
                    {
                        _resultExecutedContext = new ResultExecutedContext(
                            _resultExecutingContext,
                            _filters,
                            _resultExecutingContext.Result,
                            _module)
                        {
                            Canceled = true,
                        };
                    }
                }
                else if (item.Filter != null)
                {
                    item.Filter.OnResultExecuting(_resultExecutingContext);

                    if (_resultExecutingContext.Cancel == true)
                    {
                        _resultExecutedContext = new ResultExecutedContext(
                            _resultExecutingContext,
                            _filters,
                            _resultExecutingContext.Result,
                            _module)
                        {
                            Canceled = true,
                        };
                    }
                    else
                    {
                        item.Filter.OnResultExecuted(await InvokeResultFilterAsync());
                    }
                }
                else
                {
                    _cursor.Reset();

                    // The empty result is always flowed back as the 'executed' result
                    if (_resultExecutingContext.Result == null)
                    {
                        _resultExecutingContext.Result = new EmptyResult();
                    }

                    await _resultExecutingContext.Result.ExecuteResultAsync(Context);

                    Debug.Assert(_resultExecutedContext == null);
                    _resultExecutedContext = new ResultExecutedContext(
                        _resultExecutingContext,
                        _filters,
                        _resultExecutingContext.Result,
                        _module);
                }
            }
            catch (Exception exception)
            {
                _resultExecutedContext = new ResultExecutedContext(
                    _resultExecutingContext,
                    _filters,
                    _resultExecutingContext.Result,
                    _module)
                {
                    ExceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception)
                };
            }

            return _resultExecutedContext;
        }

        private IFilterMetadata[] GetFilters()
        {
            var filterDescriptors = Context.ActionDescriptor.FilterDescriptors;
            var items = new List<FilterItem>(filterDescriptors.Count);
            for (var i = 0; i < filterDescriptors.Count; i++)
            {
                items.Add(new FilterItem(filterDescriptors[i]));
            }

            var context = new FilterProviderContext(Context, items);
            for (var i = 0; i < _filterProviders.Count; i++)
            {
                _filterProviders[i].OnProvidersExecuting(context);
            }

            for (var i = _filterProviders.Count - 1; i >= 0; i--)
            {
                _filterProviders[i].OnProvidersExecuted(context);
            }

            var count = 0;
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Filter != null)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                return new IFilterMetadata[0];
            }
            else
            {
                var filters = new IFilterMetadata[count];
                for (int i = 0, j = 0; i < items.Count; i++)
                {
                    var filter = items[i].Filter;
                    if (filter != null)
                    {
                        filters[j++] = filter;
                    }
                }

                return filters;
            }
        }

        private struct FilterCursor
        {
            private int _index;
            private readonly IFilterMetadata[] _filters;

            public FilterCursor(int index, IFilterMetadata[] filters)
            {
                _index = index;
                _filters = filters;
            }

            public FilterCursor(IFilterMetadata[] filters)
            {
                _index = 0;
                _filters = filters;
            }

            public void Reset()
            {
                _index = 0;
            }

            public FilterCursorItem<TFilter, TFilterAsync> GetNextFilter<TFilter, TFilterAsync>()
                where TFilter : class
                where TFilterAsync : class
            {
                while (_index < _filters.Length)
                {
                    var filter = _filters[_index] as TFilter;
                    var filterAsync = _filters[_index] as TFilterAsync;

                    _index += 1;

                    if (filter != null || filterAsync != null)
                    {
                        return new FilterCursorItem<TFilter, TFilterAsync>(_index, filter, filterAsync);
                    }
                }

                return default(FilterCursorItem<TFilter, TFilterAsync>);
            }
        }

        private struct FilterCursorItem<TFilter, TFilterAsync>
        {
            public readonly int Index;
            public readonly TFilter Filter;
            public readonly TFilterAsync FilterAsync;

            public FilterCursorItem(int index, TFilter filter, TFilterAsync filterAsync)
            {
                Index = index;
                Filter = filter;
                FilterAsync = filterAsync;
            }
        }
    }
}