﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  internal static class Extensions
  {
    public static LinkedListNode<T> Next<T>(this LinkedListNode<T> node)
    {
      return node.Next ?? node.List.First;
    }

    public static LinkedListNode<T> Previous<T>(this LinkedListNode<T> node)
    {
      return node.Previous ?? node.List.Last;
    }
  }
}