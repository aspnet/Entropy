# Generic Controllers
  
This sample shows extensibility that will create additional controller types based on a `GenericController<>` base class and a set of known *entity types*.

These *generic controllers* act as a fallback for any controllers defined in a conventional way. For instance if `SprocketController` is defined by the application, then `GenericController<Sprocket>` will not be used.
