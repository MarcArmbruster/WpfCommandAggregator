# WPF Command Aggregator
The WPF Command Aggregator is a solution to reduce WPF command definitions to an absolute minimum of code lines (only one per command).
In addition, a BaseViewModel class (BaseVm) with an integrated command aggregator instance and the DependsOn attribute is supported.

Latest stable version is available as a nuGet package:<br/>
[nuGet](https://www.nuget.org/packages/WPFCommandAggregator/)

There are also a UWP and a WPF.NET (.NET6 or higher) version of the CommandAggregator:<br/>

.NET (6 or higher)<br/>
[gitHub: WPF Command Aggregator Core](https://github.com/MarcArmbruster/WpfCommandAggregator.Core)<br/>
[nuGet: WPF Command Aggregator Core](https://www.nuget.org/packages/WpfCommandAggregator.Core/)

UWP:<br/>
[gitHub: UWP Command Aggregator](https://github.com/MarcArmbruster/UwpCommandAggregator)<br/>
[nuGet: UWP Command Aggregator](https://www.nuget.org/packages/UwpCommandAggregator/)

## Versions
- 2.0.0.0
    - InitCommands method within BaseVm was set from abstract to virtual
    - Target Framework set to .Net 4.8 (or higher). 
    If .Net 4.6.2 or .Net 4.7 must be supported, you can still use version 1.6.1.0 
- 1.6.1.0
  - some internal code optimizations
  - Minimum .net Framework Version upgraded from 4.5.2 to 4.6.2!
    If you still need running the component on .net Framework 4.5.2 please still use the stable and proved version 1.6.0.0
- 1.6.0.0
  - new: ObservableCollectionExt extends ObservableCollection with fast 
    AddRange, RemoveItems methods and also a Replace method
    The class is placed in the namespace System.Collections.ObjectModel.
  - new: Attached properties for managing focus and closing windows via view model properties.
- 1.5.0.0 
  - new: Factory extended to enable registration/unregistration of a custom command aggregator implementation. 
  - new: Automatic values storage added (no more private backing fields required for bindable properties): incl. Set and Get methods.
  - change: SetPropertyValue methods checks for real changes - only than a notification will be raised.
- 1.4.0.0 : 
  - new: Pre and post delegates for BaseVm.AddOrSetCSetPropertyValue method and BaseVm.SuppressNotification property. New overload for ICommandAggregator.AddOrSetCommand
- 1.3.0.0 : 
  - new: DependsOn Attribute added and BasVm class optimzed; Target framework version set to 4.5.2
- 1.2.0.0 : 
  - new: Pre- and post action delegates added; Target framework version set to 4.5.1 
- 1.1.0.0 : 
  - new: HierarchyCommand added; Target framework version set to 4.5.1 
- 1.0.0.0 : 
  - WPF Command Aggregator
            

## The Background

The last few years I worked on many WPF projects (MVVM) with many views and thereof with many command objects. It was quite boring to write similar code for every command definition. The well known RelayCommand (Josh Smith) helps a lot but there is still similar code for every command definition.
- private ICommand member
- public ICommand getter incl. creation logic

If you have to create views with a lot of functionality (many customers still like screens full of data and functionalities), for example ToolBars and ContextMenus also a lot of commands has to be defined.

This leads to a great amount of code lines with very similar structure. 

**Example (without Command Aggregator):**
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

**Example (with Command Aggregator):**
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

   protected virtual void InitCommands() 
   {
   }

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

## Version 2.0.0.0: virtual InitCommands
The method InitCommands of the base view model (BaseVm) was changed from abstratc definition to virtual implementation.
Therefore you do not need to override the method if you don't need any commands in your view model.

## Version 1.1.0.0: hierarchy command

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

**A short example:**
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

## Version 1.2.0.0: pre- and post action delegates

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

## Version 1.3.0.0: the 'DependsOn' attribute

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

## Version 1.4.0.0: 'SetPropertyValue'

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


## Version 1.5.0.0: automatic values storage, custom aggregators and notification optimization

The version 1.5.0.0 contains two new features and one optimization.

1. The optimization: Previously, a **notification** was always triggered for value assignments to a (bindable) property. As of version 1.5.0.0, this only happens if the value has effectively changed - in other words, if the content has changed.   
2. The first new feature: automatic values storage<br/>
The base ViewModel now has a **value storage** in which the values for the (bindable) properties are stored. This means that no more private fields have to be declared in the ViewModels. This advantage brings with it a small disadvantage: when reading the values a 'cast' is necessary. For properties that are read very frequently, you should therefore use this with caution to avoid performance disadvantages. Of course, you can continue to work with private fields in the future. A mixture of both concepts within a ViewModel is not recommended with regard to the readability of the code (but is possible without problems).<br/>
    ```C#	
    public bool CanSave
    {
        // using NO private field -> using automatic values storage (from base class).
        get => this.GetPropertyValue<bool>();
        set => this.SetPropertyValue<bool>(value);                          
    }
    ```
3.  The second new feature: **custom command aggregator** implmentations<br/>
It is now possible to use your own implementation of the Command Aggregator. For this purpose this implementation can be registered at the factory class. Until a possible deregistration the own implementation will be used by the factory. The condition is that the implementation has a parameterless constructor.<br/>
**register:** (to use an own/custom implementation)
    ```C#	
    CommandAggregatorFactory.RegisterAggreagtorImplementation<OwnAggregator>();
    // now the own/custom aggregator will be used!
    ```
    **unregister** (to change the factory behavior during runtime - otherwise not necessary):
    ```C#	
    CommandAggregatorFactory.UnregisterAggreagtorImplementation<OwnAggregator>();
    // now the default aggregator will be used again!
    ```

## Version 1.6.0.0: ObservableCollectionExt, attached properties Focus and WindowCloser

The version 1.6.0.0 contains three new features.

1. The class ObservableCollectionExt extends the well-known framework class ObservableCollection with fast methods to add and reduce multiple elements. A replace method has also been created that exchanges elements in the collection.<br/>   
    ```C#	
    public ObservableCollectionExt<Person> Persons
    {
        get { return this.GetPropertyValue<ObservableCollectionExt<Person>>(); }
        private set { this.SetPropertyValue<ObservableCollectionExt<Person>>(value); }
    }

    private void Test()
    {
        this.Persons.AddRange(this.allPersons);
        this.Persons.RemoveItems(this.allPersons);
        this.Persons.Replace(allPersons.First(), new Person { Name = "Gerhard", Age = 27 });
    }
    ```
2. Attached property to set/remove focus from a Control.<br/>

    ```XAML	
     <TextBox
                ap:Focused.Focused="{Binding Path=FirstValueHasFocus, UpdateSourceTrigger=PropertyChanged, Mode=TwoWay}" 
                Grid.Row="1"
                Grid.Column="0">
     </TextBox>
    ```
3.  The third new feature: is an attached property to close views from the view model.<br/>

    ```C#	
    private void CloseWindow()
    {
        // setting the value to true (or false) will close the window using this instance as its DataContext
        // and uses the attached property.
        // The property itself is defined in class BaseVm.
        this.WindowResult = true;
    }
    ```
    
    ```XAML	
    <Window
    x:Class="CommandAggregatorExample.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ap="clr-namespace:WPFCommandAggregator.AttachedProperties;assembly=WPFCommandAggregator"
    ap:WindowCloser.WindowResult="{Binding WindowResult, UpdateSourceTrigger=PropertyChanged, Mode=TwoWay}"
    Title="Command Aggregator Example"
    Width="600"
    Height="600"
    WindowStartupLocation="CenterScreen"/>
    ```

(see also the example solution (source code))
