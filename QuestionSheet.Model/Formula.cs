using CodeExecutor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestionSheet.Model
{
    public class Formula
    {
        public IList<Parameter> Parameters { get; set; }
        public string OutputDisplay { get; set; }
        public string Code { get; set; }

        public Formula() 
        {
            Parameters = new List<Parameter>();
            OutputDisplay = string.Empty;
            Code = string.Empty;
        }

        public double Execute(double[] parameters) {
            StringBuilder builder = new StringBuilder();
            builder.Append("public class Calculator{public static double Calculate(){");

            for (int i = 0; i < parameters.Length; i++)
            {
                builder.Append($"double {Parameters[i].VariableName}={parameters[i].ToString().Replace(",", ".")}D;");
            }

            builder.Append($"return {Code};");
            builder.Append("}}");

            return new Executor().Execute(builder.ToString());
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Parameters.Count);
            foreach (Parameter parameter in Parameters) {
                writer.Write(parameter.DisplayName);
                writer.Write(parameter.VariableName);
            }

            writer.Write(OutputDisplay);
            writer.Write(Code);
        }
        public static Formula FromBinaryReader(BinaryReader reader)
        {
            Formula output = new Formula();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++) {
                output.Parameters.Add(new Parameter(reader.ReadString(), reader.ReadString()));
            }

            output.OutputDisplay = reader.ReadString();
            output.Code = reader.ReadString();

            return output;
        }
    }

    public struct Parameter 
    { 
        public string DisplayName { get; set; }
        public string VariableName { get; set; }

        public Parameter(string displayName, string variableName) 
        {
            DisplayName = displayName;
            VariableName = variableName;
        }
    }
}
