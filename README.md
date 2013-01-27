BindingEngine
=============

Custom BindingEngine that support Winform, WPF and other scenes. 
The goal of BindingEngine is making your ViewModel everywhere.

The original implementation of Winform binding is heavy and not powerful enough than WPF.
It does not support ICommand, Recursive Binding, etc.

The binding can be analyzed for 3 parts:
1. When.   
   What trigger the binding update? Usually the binding is updated via INotifyPropertyChanged's PropertyChanged event in ViewModel scenario.
2. Direction
   Direction of the Data Flow. OneWay, TwoWay or OneWayToSource?
3. Update
   After binding is triggered, the detail update strategy for different binding.
   The binding can work on pure Property, Collection, ICommand and Method.

The use of BindingEngine:
BindingEngine.SetXXXBinding( source, sourceProp, target, targetProp ), XXX can be Property, Collection, Command and Method.

BindingEngine.SetPropertyBinding(nameTextbox, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel.CurrentPerson.Name)
             .SetMode(BindMode.TwoWay)
             .AttachSourceEvent("TextChanged");

SetPropertyBinding indicates it's a Property Binding;
SetMode indicates it's a TwoWay binding;
AttactSourceEvent indicates the binding is triggered when TextChanged event occured on the source-->nameTextbox. 

BindingEngine.SetMethodBinding(viewModelLabel, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel)
             .AttachTargetMethod<DataWarehouse>(o => o.MainViewModel, "GetHashCode");
This binding means the Label-->viewModelLabel.Text is bind to the MainViewModel GetHashCode method.

Use the BindingEngine, you can switch the view from WPF to Winform without changing the codes of ViewModel.
The BindingEngine can be extend to support more functions, like conversion to auto apply event trigger for different type.
I don't like the heavy codes to do the simple thing, there are just two files : BindingEngine and EmitEngine, you can easily integrate the codes in your solution.
Have fun with it and appreciate for any suggestion. ^_^

yohan zhou
mailto:yohan.zhou@gmail.com
