# WPF Command Aggregator
The WPF Command Aggregator is a solution to reduce WPF command definitions to an absolute minimum of code lines (only one per command).
In addition, a BaseViewModel class (BaseVm) with an integrated command aggregator instance and the DependsOn attribute is supported.

Since 07/2018 there is also a UWP version of the CommandAggregator: [UWP Command Aggregator](https://github.com/MarcArmbruster/UwpCommandAggregator)

## Versions
- 1.0.0.0 : WPF Command Aggregator
- 1.1.0.0 : HierarchyCommand added; Target framework version set to 4.5.1 
- 1.2.0.0 : Pre- and post action delegates added; Target framework version set to 4.5.1 
- 1.3.0.0 : DependsOn Attribute added and BasVm class optimzed; Target framework version set to 4.5.2
- 1.4.0.0 : Pre and post delegates for BaseVm.AddOrSetCSetPropertyValue method and BaseVm.SuppressNotification property. New overload for ICommandAggregator.AddOrSetCommand

## The Background

The last few years I worked on many WPF projects (MVVM) with many views and thereof with many command objects. It was quite boring to write similar code for every command definition. The well known RelayCommand (Josh Smith) helps a lot but there is still similar code for every command definition.
- private ICommand member
- public ICommand getter incl. creation logic

If you have to create views with a lot of functionality (many customers still like screens full of data and functionalities), for example ToolBars and ContextMenus also a lot of commands has to be defined.

This leads to a great amount of code lines with very similar structure. 

**Example (+without+ Command Aggregator):**
```C#
private ICommand printCommand;
public ICommand PrintCommand
{
     get
     {
          if (this.printCommand == null)
          {
              this.printCommand = new RelayCommand(p1 => Print(p1), p2 => CanPrint);
          }
          return this.printCommand;
      }
}
```

All we want to tell is: _PrintCommand_ executes the _Print_ method if it is allowed (_CanPrint_ property).

The WPF Command Aggregator is a solution to reduce the command definitions to a very short and easy to read line of code within a view model class.

**Example (+with+ Command Aggregator):**
```C#
this.CmdAgg.AddOrSetCommand("Print", new RelayCommand(p1 => Print(p1), p2 => CanPrint));
```

This is an reduction of about 10 lines (!!!) and (in my opinion) a very easy to read command definition.
How can this be achieved and how we can use it for bindings (XAML)?

## The Implementation

The Command Aggreagtor is a class containing a dictionary (ConcurrentDictionary) to hold commands identified by a string (command name):
```C#
private readonly ConcurrentDictionary<string, ICommand> commands;

public CommandAggregator()
{
   commands = new ConcurrentDictionary<string, ICommand>();
}
```

A method called _AddOrSetCommand_ (overloaded) provides the registration of new commands:
```C#
public void AddOrSetCommand(string key, ICommand command)
{
   if (this.commands.Any(k => k.Key == key))
   {
      this.commands[key] = command;
   }
   else
   {
      this.commands.AddOrUpdate(key, command, (exkey, excmd) => command);
    }
}

public void AddOrSetCommand(string key, Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
{
   if (this.commands.Any(k => k.Key == key))
   {
      this.commands[key] = new RelayCommand(executeDelegate, canExecuteDelegate);
   }
   else
   {
      ICommand command = new RelayCommand(executeDelegate, canExecuteDelegate);
      this.commands.AddOrUpdate(key, command, (exkey, excmd) => command);
   }
}
```

So, we have the functionaliy to collect commands within a CommandAggregator class, but how we can use it - especially in Bindings?
First of all my BaseViewModel class is provided with one CommandAggregator instance and an abstract 
method called _InitCommands_.

```C#
public abstract class BaseVm : INotifyPropertyChanged
{
   private CommandAggregator cmdAgg = new CommandAggregator();
   public CommandAggregator CmdAgg
   {
       get
       {
           return this.cmdAgg;
        }
   }

   protected abstract void InitCommands();

   protected BaseVm()
   {
        this.InitCommands();
   }

   // ... further elements of the base view model
}
```

The abstract _InitCommands_ method is the place where the commands will be registered/defined within each view model.
```C#
public class MainVm : BaseVm
{
   protected sealed override void InitCommands()
   {
      this.CmdAgg.AddOrSetCommand("Print", new RelayCommand(p1 => Print(p1), p2 => CanPrint));
   }

   // ... more view model code ...
}
```

But there is still one problem left: How do we bind the commands?
At this point an indexer can help us. Indexers can be used in Bindings for a OneWay binding. Commands do not use TwoWay bindings, so an indexer within the CommandAggregator class enables us to establish a command binding in XAML.

```C#
   public ICommand this[string key]
   {
       get
       {
           // The indexer is using the GetCommand-Method
           return this.GetCommand(key);
       }
    }

    public ICommand GetCommand(string key)
    {
       if (this.commands.Any(k => k.Key == key))
       {
          return this.commands[key];
       }
       else
       {
          // Empty command (to avoid null reference exceptions)
          return new RelayCommand(p1 => { });
        }  
      }
```

## Usage

In XAML we can use the CommandAggregator instance of the view model like this:

```C#
<Button Content="Print" Command="{Binding CmdAgg[Print]}" />
```

Indexer binding works with square brackets and the name of the registered command - you do not need any quotation marks!

The WPF Command Aggregator is working well in my current and previous projects. I was able to reduce the lines of code for command definitions for more than 2000 (within my current project) without loosing any functionality! Each command definition/registration is reduced to exact one line of code.

Thanks to Gerhard Ahrens for all the discussions and testing after work time!

## Hierarchy Command (Version 1.1.0.0)

With version 1.1.0.0 the target framework version was set to 4.5.1.
This version also comes with a new feature - I called it the **HierarchyCommand**.

This command also implements the ICommand interface and derives from the RelayCommand class.
A HierarchyCommand can have child commands. Therefore the HierarchyCommand depends on the child commands. Fore example: a SaveAll command depends on the save commands of all child commands.
To configure the dependency of the HierarchyCommand startegies can be used for Execute and CanExecute:

**HierarchyCanExecuteStrategy**
* DependsOnMasterCommandOnly
* DependsOnAllChilds
* DependsOnAtLeastOneChild

**HierarchyExecuteStrategy**
* MasterCommandOnly
* AllChildsOnly
* MasterAndAllChilds

With these values many business cases can be realized.

+A short example:+
```C#
   // Adding a hierarchy command
   ICommand save1Cmd = new RelayCommand(p1 => Save1(), p2 => this.CanSave1);
   ICommand save2Cmd = new RelayCommand(p1 => Save2(), p2 => this.CanSave2);

   this.CmdAgg.AddOrSetCommand("SaveCmd1", save1Cmd);
   this.CmdAgg.AddOrSetCommand("SaveCmd2", save2Cmd);

   HierarchyCommand saveAllCmd = new HierarchyCommand(
                p1 => {  },  -- no Execute required due to HierarchyExecuteStrategy
                p2 => null,  -- CanExecute not required due to HierarchyCanExecuteStrategy
                HierarchyExecuteStrategy.AllChildsOnly,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

   saveAllCmd.AddChildsCommand(new List<ICommand> { save1Cmd, save2Cmd });
   this.CmdAgg.AddOrSetCommand("SaveAll", saveAllCmd); 
```

## Pre- and post action delegates (Version 1.2.0.0)

This version comes with a new feature - the **pre - and post action delegates**.

The RelayCommand was extended by two delegates "PreActionDelegate" and "PostActionDelegate".
This enables you to add logic to run before and after the command execution delegate runs.
Therefore these delegates enables you to "inject" cross cutting concerns without the need to change your execution delegate logic.

+A short example (performance messurement):+
```C#
            this.CmdAgg.AddOrSetCommand(
                                        "Print", 
                                        new RelayCommand(
                                            p1 => MessageBox.Show("Print called"), 
                                            p2 => true, 
                                            this.performanceChecker.Start, 
                                            this.performanceChecker.Stop));
```

The performanceChecker instance is only a wrapper class to start and stop a stopwatch instance and writes the ellapsed time to the debug window. You can use any Action delegate.

When you like to set/change/remove these actions dynamically you can use the corresponding methods of the RelayCommand class.
```C#
            RelayCommand cmd = this.CmdAgg["Print"] as RelayCommand;
            if (cmd != null)
            {
                 cmd.OverridePostActionDelegate(null);
                 cmd.OverridePreActionDelegate(null);
            }
```

## The DependsOn attribute (Version 1.3.0.0)

Sometimes properties of a view model class depends on others. A simlpe example is the calculation of the sum of two input values.
In a classic way the code of the view model for this purpose could look like that:
```C#
        public decimal? FirstInput
        {
            get => this.firstInput;
            set
            {
                this.firstInput = value;
                this.NotifyPropertyChanged(nameof(FirstInput));
                this.NotifyPropertyChanged(nameof(Result));
            }
        }

        public decimal? SecondInput
        {
            get => this.secondInput;
            set
            {
                this.secondInput = value;
                this.NotifyPropertyChanged(nameof(SecondInput));
                this.NotifyPropertyChanged(nameof(Result));
            }
        }

        public decimal? Result
        {
            get => this.firstInput + this.secondInput;
        }

```

Wouldn't it be better to define/see the dependencies between the properties at one place: in my opinion the Result property would be a very good place.
Therefore, with the DependsOn attribute and the optimzed BaseVM class this can be achieved easier and better to read:
```C#
        public decimal? FirstInput
        {
            get => this.firstInput;
            set => this.SetPropertyValue(ref this.firstInput, value);
        }

        public decimal? SecondInput
        {
            get => this.secondInput;
            set => this.SetPropertyValue(ref this.secondInput, value);
        }

        [DependsOn(nameof(FirstInput), nameof(SecondInput))]
        public decimal? Result
        {
            get => this.firstInput + this.secondInput;
        }
```
Now, the attribute defines the dependencies and the BaseVM class will do the rest for you (notifications).

## SetPropertyValue (Version 1.4.0.0)

In version 1.3.0.0 the SetPropertyValue method was introduced. 
In some cases in could be helpful to additionally execute some 
code lines before or - normally - after the set and notification is done.
In version 1.4.0.0 the possibility to additionally define two action delegates is implemented.
The following example shows it based on two simple methods of a performance checker instance.

```C#
        public bool ExampleProperty
        {
            get => this.exampleProperty;
            set => this.SetPropertyValue(
                ref this.exampleProperty, 
                value,
                () => this.performanceChecker.Start(),
                () => this.performanceChecker.Stop());
        }
```

Furthermore the BaseVm class has a new protected property called SuppressNotifications.
By default this ist set to false. If you want to suppress notifications (e.g. to temporarily improve performance by decoupling from UI)

```C#
    this.SuppressNotifications = true;
``` 

The command aggregator interface itself was also extended by a new overload for the AddOrSetCommand method.
The definition for always executable commands  can shortly defined by omitting the can execute delegate.

The definition
```C#	
    this.CmdAgg.AddOrSetCommand("Exit", new RelayCommand(p1 => MessageBox.Show("Exit called"), p2 => true));
```
can now be simplified with the following one:

```C#	
    this.CmdAgg.AddOrSetCommand("Exit", new RelayCommand(p1 => MessageBox.Show("Exit called")));
```

(see also the example solution (source code))
