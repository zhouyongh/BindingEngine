<strong>BindingEngine</strong></br>  
===========    </br>

Custom BindingEngine that support Winform, WPF and other scenes. </br>  
The goal of BindingEngine is making your ViewModel everywhere.  </br> 

The original implementation of Winform binding is heavy and not powerful enough than WPF.</br>   
It does not support ICommand, Recursive Binding, etc. </br>  

The binding can be analyzed for 3 parts:</br>  
<strong>>1. When</strong></br>
   What trigger the binding update? Usually the binding is updated via INotifyPropertyChanged's PropertyChanged event in ViewModel scenario.</br>   
<strong>>2. Direction</strong></br>    
   Direction of the Data Flow. OneWay, TwoWay or OneWayToSource?</br>   
<strong>>3. Update</strong></br>    
   After binding is triggered, the detail update strategy for different binding.</br>
   The binding can work on pure Property, Collection, ICommand and Method.</br>

The use of BindingEngine:</br>    
<pre><code>BindingEngine.SetXXXBinding( source, sourceProp, target, targetProp )</code></pre>, XXX can be Property, Collection, Command and Method.

<pre><code>BindingEngine.SetPropertyBinding(nameTextbox, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel.CurrentPerson.Name)    
             .SetMode(BindMode.TwoWay)    
             .AttachSourceEvent("TextChanged");</code></pre>
SetPropertyBinding indicates it's a Property Binding;</br>      
SetMode indicates it's a TwoWay binding;</br>    
AttactSourceEvent indicates the binding is triggered when TextChanged event occured on the source-->nameTextbox. </br>   
<pre><code>BindingEngine.SetMethodBinding(viewModelLabel, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel)    
             .AttachTargetMethod<DataWarehouse>(o => o.MainViewModel, "GetHashCode");</code></pre>    
This binding means the Label-->viewModelLabel.Text is bind to the MainViewModel GetHashCode method.</br>  

Use the BindingEngine, you can switch the view from WPF to Winform without changing the codes of ViewModel.     
The BindingEngine can be extend to support more functions, like conversion to auto apply event trigger for different type.     
I don't like the heavy codes to do the simple thing, there are just two files : BindingEngine and EmitEngine, you can easily integrate the codes in your solution.    
Have fun with it and appreciate for any suggestion. ^_^ </br>

<strong>yohan zhou</strong>   
mailto:yohan.zhou@gmail.com
