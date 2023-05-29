using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class DisableMultieditSupportAttribute : Attribute {}
