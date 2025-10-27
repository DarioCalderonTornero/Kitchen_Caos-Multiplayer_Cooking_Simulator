using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgress  {

    public event EventHandler<OnProgressChangedEventsArgs> OnProgressChanged;
    public class OnProgressChangedEventsArgs : EventArgs
    {
        public float progressNormalized;
    }
}
   

