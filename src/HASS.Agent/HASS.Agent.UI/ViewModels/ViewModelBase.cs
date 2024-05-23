using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json.Bson;

namespace HASS.Agent.UI.ViewModels;
public abstract class ViewModelBase : ObservableRecipient
{
    protected DispatcherQueue _dispatcherQueue;
    protected ViewModelBase(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue;
    }

    protected void RaiseOnPropertyChanged(string propertyName)
    {
        _dispatcherQueue.TryEnqueue(() => OnPropertyChanged(propertyName));
    }

    protected void RunOnDispatcher(DispatcherQueueHandler handler)
    {
        _dispatcherQueue.TryEnqueue(handler);
    }
}
