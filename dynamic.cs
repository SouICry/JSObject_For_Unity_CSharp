﻿using System;
using System.Collections.Generic;

/// <summary>
/// <para>A dynamic type implementation for JavaScript style objects in Unity C# (with no System.Dynamic support) 
/// 
///       Implements some functionality of a dynamic type with the class dynamic since Unity does not support dynamic types. 
///       Designed to work with JSONParser (JSONObject parser specifically) as is, but expandable.
/// 
///       Supports string, float, int, bool, List(dynamic) and List{string, dynamic}. Doubles are automatically converted to floats. Nests infinitely. </para>
/// <para>Usage: </para>
/// <para>Values or references can be directly assigned from or to this dynamic object. Loosely typed, so most cast errors only show up at run time as an exception. </para>
/// <para>    dynamic d = "squid"; </para>
/// <para>    string s = d;                    //No cast necessary </para>
/// <para>    d = 1;                           //Converted to int </para>
/// <para>    s = (d + 4) % 5;                 //Supports standard math and string operations + - * / % and +(concat) </para>
/// <para>    float f = d * 5;                 //and type conversions </para>
/// <para>    Console.Write(d.ToString());     //Unfortunately ToString() is necessary due to a C# bug in Console. Other uses, like Debug.Log(d), Debug.Write(d), work perfectly. </para>
/// 
/// <para> Some common methods and properties are implemented for List and Dictionary. The rest can be accessed by assigning it to a correct type reference. </para>
/// <para>     dynamic d = new Dictionary{string, dynamic}();                   </para>
/// <para>     d.Add("key", new List{dynamic});                                 </para>
/// <para>     d["key"].Count;                                          //0  Gets the Count for the list </para>
/// <para>     d.ContainsKey("key");                                    //true </para>
/// <para>     d["key"] = 15;                                           //Converts the list in the dictionary to int with value 15 </para>
/// <para>     Dictionary{string, dynamic}.KeyCollection keys = d.Keys; //Gets the usual list of keys </para>
/// <para>     Dictionary{string, dynamic} dict = d;                    //Gets the actual dictionary </para>
/// <para>     dict.GetEnumerator();                                    //Other methods available after conversion </para>
/// </summary>

public class dynamic
{
    enum Type { STRING, FLOAT, INT, BOOL, LIST_DYNAMIC, DICTIONARY_STRING_DYNAMIC, DOUBLE }

    private Type type;
    private readonly string stringValue;
    private readonly float floatValue;
    private readonly int intValue;
    private readonly bool boolValue;
    private readonly List<dynamic> listValue;
    private readonly Dictionary<string, dynamic> dictionaryValue;


    //Constructors

    public dynamic(double value)
    {
        type = Type.DOUBLE;
        stringValue = null;
        floatValue = (float)value;
        intValue = 0;
        boolValue = false;
        listValue = null;
        dictionaryValue = null;
    }
    public dynamic(string value)
    {
        type = Type.STRING;
        stringValue = value;
        floatValue = 0;
        intValue = 0;
        boolValue = false;
        listValue = null;
        dictionaryValue = null;
    }
    public dynamic(int value)
    {
        type = Type.INT;
        stringValue = null;
        floatValue = 0;
        intValue = value;
        boolValue = false;
        listValue = null;
        dictionaryValue = null;
    }
    public dynamic(float value)
    {
        type = Type.FLOAT;
        stringValue = null;
        floatValue = value;
        intValue = 0;
        boolValue = false;
        listValue = null;
        dictionaryValue = null;
    }
    public dynamic(bool value)
    {
        type = Type.BOOL;
        stringValue = null;
        floatValue = 0;
        intValue = 0;
        boolValue = value;
        listValue = null;
        dictionaryValue = null;
    }
    public dynamic(List<dynamic> value)
    {
        type = Type.LIST_DYNAMIC;
        stringValue = null;
        floatValue = 0;
        intValue = 0;
        boolValue = false;
        listValue = value;
        dictionaryValue = null;
    }
    public dynamic(Dictionary<string, dynamic> value)
    {
        type = Type.DICTIONARY_STRING_DYNAMIC;
        stringValue = null;
        floatValue = 0;
        intValue = 0;
        boolValue = false;
        listValue = null;
        dictionaryValue = value;
    }

    //Implicit setters

    public static implicit operator dynamic(double value) { return new dynamic(value); }
    public static implicit operator dynamic(string value) { return new dynamic(value); }
    public static implicit operator dynamic(float value) { return new dynamic(value); }
    public static implicit operator dynamic(int value) { return new dynamic(value); }
    public static implicit operator dynamic(bool value) { return new dynamic(value); }
    public static implicit operator dynamic(List<dynamic> value) { return new dynamic(value); }
    public static implicit operator dynamic(Dictionary<string, dynamic> value) { return new dynamic(value); }


    //Implicit getters

    public static implicit operator string (dynamic v)
    {
        if (v.type == Type.STRING) return v.stringValue;
        else if (v.type == Type.FLOAT || v.type == Type.DOUBLE) return v.floatValue.ToString();
        else if (v.type == Type.INT) return v.intValue.ToString();
        else if (v.type == Type.BOOL) return v.boolValue.ToString();
        else throw new InvalidCastException();
    }
    public static implicit operator float (dynamic v)
    {
        if (v.type == Type.FLOAT || v.type == Type.DOUBLE) return v.floatValue;
        else if (v.type == Type.INT) return v.intValue;
        else throw new InvalidCastException();
    }
    public static implicit operator int (dynamic v) { if (v.type == Type.INT) return v.intValue; else throw new InvalidCastException(); }
    public static implicit operator bool (dynamic v) { if (v.type == Type.BOOL) return v.boolValue; else throw new InvalidCastException(); }
    public static implicit operator List<dynamic>(dynamic v) { if (v.type == Type.LIST_DYNAMIC) return v.listValue; else throw new InvalidCastException(); }
    public static implicit operator Dictionary<string, dynamic>(dynamic v) { if (v.type == Type.DICTIONARY_STRING_DYNAMIC) return v.dictionaryValue; else throw new InvalidCastException(); }


    //Operators override

    public static dynamic operator +(dynamic first, dynamic second)
    {
        if (first.type == Type.INT && second.type == Type.INT)
        {
            return new dynamic(first.intValue + second.intValue);
        }
        else if ((first.type == Type.FLOAT || first.type == Type.DOUBLE) && (second.type == Type.FLOAT || second.type == Type.DOUBLE))
        {
            return new dynamic(first.floatValue + second.floatValue);
        }
        else if (first.type == Type.INT && (second.type == Type.FLOAT || second.type == Type.DOUBLE))
        {
            return new dynamic(first.intValue + second.floatValue);
        }
        else if ((first.type == Type.FLOAT || first.type == Type.DOUBLE) && second.type == Type.INT)
        {
            return new dynamic(first.floatValue + second.intValue);
        }
        else if (first.type == Type.STRING || second.type == Type.STRING)
        {
            return first.ToString() + second.ToString();
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }

    public static dynamic operator -(dynamic first, dynamic second)
    {
        if (first.type == Type.INT && second.type == Type.INT)
        {
            return new dynamic(first.intValue - second.intValue);
        }
        else if ((first.type == Type.FLOAT || first.type == Type.DOUBLE) && (second.type == Type.FLOAT || second.type == Type.DOUBLE))
        {
            return new dynamic(first.floatValue - second.floatValue);
        }
        else if (first.type == Type.INT && (second.type == Type.FLOAT || second.type == Type.DOUBLE))
        {
            return new dynamic(first.intValue - second.floatValue);
        }
        else if ((first.type == Type.FLOAT || first.type == Type.DOUBLE) && second.type == Type.INT)
        {
            return new dynamic(first.floatValue - second.intValue);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }

    public static dynamic operator *(dynamic first, dynamic second)
    {
        if (first.type == Type.INT && second.type == Type.INT)
        {
            return new dynamic(first.intValue * second.intValue);
        }
        else if ((first.type == Type.FLOAT || first.type == Type.DOUBLE) && (second.type == Type.FLOAT || second.type == Type.DOUBLE))
        {
            return new dynamic(first.floatValue * second.floatValue);
        }
        else if (first.type == Type.INT && (second.type == Type.FLOAT || second.type == Type.DOUBLE))
        {
            return new dynamic(first.intValue * second.floatValue);
        }
        else if ((first.type == Type.FLOAT || first.type == Type.DOUBLE) && second.type == Type.INT)
        {
            return new dynamic(first.floatValue * second.intValue);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }

    public static dynamic operator /(dynamic first, dynamic second)
    {
        if (first.type == Type.INT && second.type == Type.INT)
        {
            return new dynamic(first.intValue / second.intValue);
        }
        else if ((first.type == Type.FLOAT || first.type == Type.DOUBLE) && (second.type == Type.FLOAT || second.type == Type.DOUBLE))
        {
            return new dynamic(first.floatValue / second.floatValue);
        }
        else if (first.type == Type.INT && (second.type == Type.FLOAT || second.type == Type.DOUBLE))
        {
            return new dynamic(first.intValue / second.floatValue);
        }
        else if ((first.type == Type.FLOAT || first.type == Type.DOUBLE) && second.type == Type.INT)
        {
            return new dynamic(first.floatValue / second.intValue);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }

    public static dynamic operator %(dynamic first, dynamic second)
    {
        if (first.type == Type.INT && second.type == Type.INT)
        {
            return new dynamic(first.intValue % second.intValue);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }

    public static dynamic operator ==(dynamic first, dynamic second)
    {
        if (first.type != second.type)
        {
            return false;
        }
        else
        {
            switch (first.type)
            {
                case Type.STRING: return first.stringValue == second.stringValue;
                case Type.FLOAT: return first.floatValue == second.floatValue;
                case Type.INT: return first.intValue == second.intValue;
                case Type.BOOL: return first.boolValue == second.boolValue;
                case Type.LIST_DYNAMIC: return first.listValue == second.listValue;
                case Type.DICTIONARY_STRING_DYNAMIC: return first.dictionaryValue == second.dictionaryValue;
                default: return false;
            }
        }
    }
    public static dynamic operator !=(dynamic first, dynamic second)
    {
        if (first.type != second.type)
        {
            return true;
        }
        else
        {
            switch (first.type)
            {
                case Type.STRING: return first.stringValue != second.stringValue;
                case Type.FLOAT: return first.floatValue != second.floatValue;
                case Type.INT: return first.intValue != second.intValue;
                case Type.BOOL: return first.boolValue != second.boolValue;
                case Type.LIST_DYNAMIC: return first.listValue != second.listValue;
                case Type.DICTIONARY_STRING_DYNAMIC: return first.dictionaryValue != second.dictionaryValue;
                default: return false;
            }
        }
    }


    //Shared methods

    public override string ToString()
    {
        switch (type)
        {
            case Type.STRING: return stringValue.ToString();
            case Type.FLOAT: return floatValue.ToString();
            case Type.INT: return intValue.ToString();
            case Type.BOOL: return boolValue.ToString();
            case Type.LIST_DYNAMIC: return listValue.ToString();
            case Type.DICTIONARY_STRING_DYNAMIC: return dictionaryValue.ToString();
            default: return "Error";
        }
    }

    /// <summary>
    /// Same as ==, checks value equality for value types and references for objects.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return this == (dynamic)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }


    //List and Dictionary method

    public int Count
    {
        get
        {
            if (type == Type.LIST_DYNAMIC)
            {
                return listValue.Count;
            }
            else if (type == Type.DICTIONARY_STRING_DYNAMIC)
            {
                return dictionaryValue.Count;
            }
            else
            {
                throw new ArgumentException("Incompatible types");
            }
        }
    }


    //List<dynamic> properties and methods

    public void Add(dynamic value)
    {
        if (type == Type.LIST_DYNAMIC)
        {
            listValue.Add(value);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }

    public dynamic this[int key]
    {
        get
        {
            if (type == Type.LIST_DYNAMIC)
            {
                return listValue[key];
            }
            else
            {
                throw new ArgumentException("Incompatible types");
            }
        }
        set
        {
            if (type == Type.LIST_DYNAMIC)
            {
                listValue[key] = value;
            }
            else
            {
                throw new ArgumentException("Incompatible types");
            }
        }
    }

    public bool Contains(dynamic value)
    {
        if (type == Type.LIST_DYNAMIC)
        {
            return listValue.Contains(value);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }


    //Dictionary properties and methods

    public Dictionary<string, dynamic>.KeyCollection Keys
    {
        get
        {
            if (type == Type.DICTIONARY_STRING_DYNAMIC)
            {
                return dictionaryValue.Keys;
            }
            else
            {
                throw new ArgumentException("Incompatible types");
            }
        }
    }

    public dynamic this[string key]
    {
        get
        {
            if (type == Type.DICTIONARY_STRING_DYNAMIC)
            {
                return dictionaryValue[key];
            }
            else
            {
                throw new ArgumentException("Incompatible types");
            }
        }
        set
        {
            if (type == Type.DICTIONARY_STRING_DYNAMIC)
            {
                dictionaryValue[key] = value;
            }
            else
            {
                throw new ArgumentException("Incompatible types");
            }
        }
    }

    public void Add(string key, dynamic value)
    {
        if (type == Type.DICTIONARY_STRING_DYNAMIC)
        {
            dictionaryValue.Add(key, value);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }

    public bool ContainsKey(string key)
    {
        if (type == Type.DICTIONARY_STRING_DYNAMIC)
        {
            return dictionaryValue.ContainsKey(key);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }

    public bool ContainsValue(dynamic value)
    {
        if (type == Type.DICTIONARY_STRING_DYNAMIC)
        {
            return dictionaryValue.ContainsValue(value);
        }
        else
        {
            throw new ArgumentException("Incompatible types");
        }
    }
}
