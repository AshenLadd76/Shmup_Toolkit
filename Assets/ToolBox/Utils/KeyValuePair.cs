using System;
using UnityEngine;

namespace Wormwood.Utils
{
   [Serializable]
   public class KeyValuePair <T,TU>
   {
      [SerializeField] private T key;

      public T Key
      {
         get => key;
         set => key = value;
      }

      [SerializeField] private TU value;
      public TU Value
      {
         get => value;
         set => this.value = value;
      }
      
      public KeyValuePair( T key, TU value )
      {
         this.key = key;
         this.value = value;
      }
   }
}
