using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

// this provides centralized access and modification to data containers
// Databases will have to be created manually if certain scripts require certain databases.
// But they can also be created automatically by creating a variable in the dialogue file
public class VariableStore
{
    private const string DEFAULT_DATABASE_NAME = "Default";

    // basically it works like this dbName.yourVariable
    public const char DATABASE_VARIABLE_ATTRIBUTE_ID = '.';
    public static readonly string REGEX_VARIABLE_IDS = @"[!]?\$[a-zA-Z0-9_.]+";
    public const char VARIABLE_ID = '$';

    // each database can have a list of variables it can work with
    public class Database
    {
        // this is a variable right here
        // we will be able to loop up a variable via a string (which is its name)
        // it can store four types of variables an int, float, bool and string

        public Database(string name)
        {
            this.name = name;
            variables = new Dictionary<string, Variable>();
        }
        public string name;
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
    }

    public abstract class Variable
    {
        // get a value
        public abstract object Get();
        // set a value
        public abstract void Set(object value);
    }

    public class Variable<T> : Variable
    {
        private T value;

        // this is the getter and setter
        // the reason we make it a Func<> here is because Func returns a value and we need it to return whatever we are retrieving which the value
        private Func<T> getter;
        // Action<> is used here because we dont need to return a value
        private Action<T> setter;

        public Variable(T defaultValue = default, Func<T> getter = null, Action<T> setter = null)
        {
            value = defaultValue;

            // if the func getter and setter are not null then the variable is a linked variable meaning we've linked it manually to something else externally
            if (getter == null)
                this.getter = () => value;
            else
                this.getter = getter;

            if (setter == null)
                this.setter = newValue => value = newValue;
            else
                this.setter = setter;
        }


        public override object Get() => getter();

        // set type T newValue
        public override void Set(object newValue) => setter((T)newValue);
    }

    // initialize our default database already
    private static Dictionary<string, Database> databases = new Dictionary<string, Database> { { DEFAULT_DATABASE_NAME, new Database(DEFAULT_DATABASE_NAME) } };
    // just grabbing the databse named "Default"
    private static Database defaultDatabase => databases[DEFAULT_DATABASE_NAME];


    // we're making the createDB and createVar funcs here return a bool is because we want to know if it succeeded doing its operations
    public static bool CreateDatabase(string name)
    {
        if (!databases.ContainsKey(name))
        {
            databases[name] = new Database(name);
            return true;
        }

        return false;

    }

    public static Database GetDatabase(string name)
    {
        if (name == string.Empty)
            return defaultDatabase;

        // if the database doesnt exist yet, then create a new one
        if (!databases.ContainsKey(name))
            CreateDatabase(name);

        // then after doing that we return that database
        return databases[name];
    }

    public static bool CreateVariable<T>(string name, T defaultValue, Func<T> getter = null, Action<T> setter = null)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        // if the variable already exists then lets return false because we already created the variable
        if (db.variables.ContainsKey(variableName))
            return false;

        db.variables[variableName] = new Variable<T>(defaultValue, getter, setter);

        return true;
    }
    public static bool TryGetValue(string name, out object variable)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (!db.variables.ContainsKey(variableName))
        {
            variable = null;
            return false;
        }

        variable = db.variables[variableName].Get();
        return true;
    }

    public static bool TrySetValue<T>(string name, T value)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (!db.variables.ContainsKey(variableName))
            return false;

        db.variables[variableName].Set(value);
        return true;

    }
    private static (string[], Database, string) ExtractInfo(string name)
    {
        // here we split the database name from the variable name if database name is present
        // the reason we do this is because there will be variables that will be part of other databases other than the default database
        string[] parts = name.Split(DATABASE_VARIABLE_ATTRIBUTE_ID);

        // the first item in the array will always be the name of the database
        // like this dbName.yourVariable
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;

        // we check here if we do the dbName.yourVariable attribute thing, because in other cases we might not put the dbName and just the var name
        // because if we just put the var name only then that variable is getting stored in the default database
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return (parts, db, variableName);

    }

    public static bool HasVariable(string name)
    {
        string[] parts = name.Split(DATABASE_VARIABLE_ATTRIBUTE_ID);
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return db.variables.ContainsKey(variableName);
    }
    public static void RemoveVariable(string name)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (db.variables.ContainsKey(variableName))
            db.variables.Remove(variableName);
    }

    // IMPORTANT THIS WILL DELETE ALL OF THE VARIABLES AND DATABASES, VIEWER DISCRETION IS ADVISED
    public static void RemoveAllVariables()
    {
        databases.Clear();
        databases[DEFAULT_DATABASE_NAME] = new Database(DEFAULT_DATABASE_NAME);
    }
    public static void PrintAllDatabases()
    {
        foreach (KeyValuePair<string, Database> dbEntry in databases)
            Debug.Log($"Database: '{dbEntry.Key}'");
    }

    public static void PrintAllVariables(Database database = null)
    {
        if (database != null)
        {
            PrintAllDatabaseVariables(database);
            return;
        }

        foreach (var dbEntry in databases)
        {
            PrintAllDatabaseVariables(dbEntry.Value);
        }

    }

    private static void PrintAllDatabaseVariables(Database database)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Database: <color=#F38544>{database.name}</color>");
        foreach (KeyValuePair<string, Variable> variablePair in database.variables)
        {
            string varName = variablePair.Key;
            object varValue = variablePair.Value.Get();

            sb.AppendLine($"\t<color=#FFB14>Variable [{varName}]</color> = <color=#FFD22D>{varValue}</color>");
        }

        Debug.Log(sb.ToString());
    }




}
