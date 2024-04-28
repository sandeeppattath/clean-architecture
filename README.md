# Clean Architecture

Clean architecture is a software architecture that helps us to keep an entire application code under control. The main goal of clean architecture is the code/logic, which is unlikely to change. It has to be written without any direct dependency. This means that if I want to change my development framework OR User Interface (UI) of the system, the core of the system should not be changed. It also means that our external dependencies are completely replaceable.

Clean architecture puts the business logic and application model at the center of the application. Instead of having business logic depend on data access or other infrastructure concerns, this dependency is inverted: infrastructure and implementation details depend on the Application Core. This functionality is achieved by defining abstractions, or interfaces, in the Application Core, which are then implemented by types defined in the Infrastructure layer. A common way of visualizing this architecture is to use a series of concentric circles, similar to an onion.

![Clean-architecture-layer](https://github.com/sandeeppattath/clean-architecture/assets/7710368/d4130a1b-b81d-415b-917b-fb736b8ebb32)

In this diagram, dependencies flow toward the innermost circle. The Application Core takes its name from its position at the core of this diagram. And you can see on the diagram that the Application Core has no dependencies on other application layers. The application's entities and interfaces are at the very center. Just outside, but still in the Application Core, are domain services, which typically implement interfaces defined in the inner circle. Outside of the Application Core, both the UI and the Infrastructure layers depend on the Application Core, but not on one another (necessarily).

![Clean-architecture](https://github.com/sandeeppattath/clean-architecture/assets/7710368/1001e998-4002-4f30-841d-f625c45af0a9)

Since the UI layer doesn't have any direct dependency on types defined in the Infrastructure project, it's likewise very easy to swap out implementations, either to facilitate testing or in response to changing application requirements. ASP.NET Core's built-in use of and support for dependency injection makes this architecture the most appropriate way to structure non-trivial monolithic applications.

For monolithic applications, the Application Core, Infrastructure, and UI projects are all run as a single application.
