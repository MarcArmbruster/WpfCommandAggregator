# The WPF Command Aggregator

## Source Code
The source code contains a solution including three projects. 
+ A very small WPF example application project
+ An unit test project.
+ The CommandAggregator project

The **WPF Command Aggregator** project including the command aggregator class, a base view model class containing an instance of the command aggregator and the well known Relay Command class (Josh Smith). The solution was developed with Visual Studio 2015 and 2017. 
In Version 1.1.0.0 a HierarchyCommand was added to enable commands depending on child commands (e.q. "SaveAll" depends on individual "Save" commands)
In Version 1.2.0.0 cross cutting actions (pre- and post action delegates) were added.
In Version 1.3.0.0 the DependsOn Attribute was introduced and the BaseVm class was optimized.


# Usage
Youn can use/import the classes directly into your solution or you can reference the binary file in your solution.
A nuGet package is und construction and available soon!

# Target Framework
- The version 1.3.0.0 binary file is provided for .net 4.5.2 and higher 
- The version 1.2.0.0 binary file is provided for .net 4.5.1 and higher 
- The version 1.1.0.0 binary file is provided for .net 4.5.1 and higher
- The version 1.0.0.0 binary file is provided for .net 4.0 and higher
But all versions are compatible (source code) with all .net 4.x. versions of the .net Framework.

See the Home.md file for a detailed explanation.
