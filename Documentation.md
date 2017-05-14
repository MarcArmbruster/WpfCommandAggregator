**The WPF Command Aggregator can be downloaded as a binary file.**

# Source Code
The source code contains a solution including three projects. 

# The +WPF Command Aggregator+ project including the command aggregator class, a base view model class containing an instance of the command aggregator and the well known Relay Command class (Josh Smith). The solution was developed with Visual Studio 2015. 
In Version 1.1.0.0 a HierarchyCommand was added to enable commands depending on child commands (e.q. "SaveAll" depends on individual "Save" commands)
In Version 1.2.0.0 cross cutting actions (pre- and post action delegates) were added.
# A very small WPF example application project
# An unit test project.

# Binary
The binary file (.net assembly / dll) represents the compiled assembly of the 'WPF Command Aggregator' project mentioned above (Version 1.2.0.0).

# Usage
Youn can use/import the classes directly into your solution or you can reference the binary file in your solution.

# Target Framework
The version 1.2.0.0 binary file is provided for .net 4.5.1 
The version 1.1.0.0 binary file is provided for .net 4.5.1 
The version 1.0.0.0 binary file is provided for .net 4.0
But all versions are compatible (source code) with all .net 4.x. versions of the .net Framework (4.0-4.6).
