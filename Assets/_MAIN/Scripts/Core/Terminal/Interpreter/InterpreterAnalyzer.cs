using Python.Runtime;
using UnityEngine;
using System;

public partial class Interpreter
{
    public bool AnalyzeCodeStructure(string code, PracticeTermConfig config, out string feedback)
    {
        feedback = "";

        if (!PythonEngine.IsInitialized)
        {
            feedback = "Python engine not initialized.";
            return false;
        }

        using (Py.GIL())
        {
            try
            {
                string analysisScript = $@"
import ast, json

tree = ast.parse({code.ToPythonLiteral()})

def has_node_type(node_type):
    return any(isinstance(n, node_type) for n in ast.walk(tree))

def has_used_variable():
    assigned = set()
    used = set()
    for node in ast.walk(tree):
        if isinstance(node, ast.Assign):
            for target in node.targets:
                if isinstance(target, ast.Name):
                    assigned.add(target.id)
        elif isinstance(node, ast.Name) and isinstance(node.ctx, ast.Load):
            used.add(node.id)
    # intersection means variable assigned and actually used
    return len(assigned & used) > 0

def has_arithmetic_ops():
    for node in ast.walk(tree):
        if isinstance(node, ast.BinOp) and isinstance(node.op, (
            ast.Add, ast.Sub, ast.Mult, ast.Div, ast.Mod, ast.FloorDiv, ast.Pow
        )):
            return True
    return False

def has_string_concatenation():
    for node in ast.walk(tree):
        if isinstance(node, ast.BinOp) and isinstance(node.op, ast.Add):
            left = node.left
            right = node.right
            # Detect if both sides are string literals
            if isinstance(left, ast.Constant) and isinstance(left.value, str):
                if isinstance(right, ast.Constant) and isinstance(right.value, str):
                    return True
            # or variable + string / string + variable
            if isinstance(left, ast.Name) and isinstance(right, ast.Constant) and isinstance(right.value, str):
                return True
            if isinstance(right, ast.Name) and isinstance(left, ast.Constant) and isinstance(left.value, str):
                return True
    return False

results = {{
    'ifStatement': has_node_type(ast.If),
    'loop': any(isinstance(n, (ast.For, ast.While)) for n in ast.walk(tree)),
    'func': has_node_type(ast.FunctionDef),
    'variable': has_used_variable(),
    'arithmetic': has_arithmetic_ops(),
    'concat': has_string_concatenation(),
}}

json.dumps(results)
";

                dynamic pyScope = Py.CreateScope();
                pyScope.Exec(analysisScript);
                string jsonResults = pyScope.Eval("json.dumps(results)").ToString();

                var results = JsonUtility.FromJson<AstResults>(jsonResults);

                feedback = "";
                bool passed = true;

                if (config.requiresIf && !results.ifStatement)
                {
                    feedback += "You need to use an if-statement.\n";
                    passed = false;
                }
                if (config.requiresLoop && !results.loop)
                {
                    feedback += "You need to use a loop.\n";
                    passed = false;
                }
                if (config.requiresFunction && !results.func)
                {
                    feedback += "You need to define a function.\n";
                    passed = false;
                }
                if (config.requiresVariable && !results.variable)
                {
                    feedback += "You need to use a variable.\n";
                    passed = false;
                }
                if (config.requiresArithmetic && !results.arithmetic)
                {
                    feedback += "You need to use an arithmetic operation (+, -, *, /, %, //, **).\n";
                    passed = false;
                }
                if (config.requiresConcatenation && !results.concat)
                {
                    feedback += "You need to concatenate strings using '+'.\n";
                    passed = false;
                }

                return passed;
            }
            catch (PythonException e)
            {
                feedback = "Code analysis failed: " + e.Message;
                return false;
            }
        }
    }

    [Serializable]
    public class AstResults
    {
        public bool ifStatement;
        public bool loop;
        public bool func;
        public bool variable;
        public bool arithmetic;
        public bool concat;
    }
}
