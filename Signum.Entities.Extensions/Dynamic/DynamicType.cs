﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Signum.Entities;
using Signum.Entities.Basics;
using Signum.Utilities;
using Signum.Utilities.ExpressionTrees;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Signum.Entities.Dynamic
{
    [Serializable, EntityKind(EntityKind.Main, EntityData.Master)]
    public class DynamicTypeEntity : Entity
    {
        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string TypeName { get; set; }
        
        [SqlDbType(Size = int.MaxValue)]
        [StringLengthValidator(AllowNulls = false, Min = 3)]
        public string TypeDefinition { get; set; }

        public DynamicTypeDefinition GetDefinition()
        {
            return JsonConvert.DeserializeObject<DynamicTypeDefinition>(this.TypeDefinition);
        }

        public void SetDefinition(DynamicTypeDefinition definition)
        {
            this.TypeDefinition = JsonConvert.SerializeObject(definition);
        }
        
        static Expression<Func<DynamicTypeEntity, string>> ToStringExpression = @this => @this.TypeName;
        [ExpressionField]
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [AutoInit]
    public static class DynamicTypeOperation
    {
        public static readonly ConstructSymbol<DynamicTypeEntity>.Simple Create;
        public static readonly ConstructSymbol<DynamicTypeEntity>.From<DynamicTypeEntity> Clone;
        public static readonly ExecuteSymbol<DynamicTypeEntity> Save;
        public static readonly DeleteSymbol<DynamicTypeEntity> Delete;
    }

    public enum DynamicTypeMessage
    {
        [Description("DynamicType '{0}' successfully saved. Go to DynamicPanel now?")]
        DynamicType0SucessfullySavedGoToDynamicPanelNow,

        [Description("Server restarted with errors in dynamic code. Fix errors and restart again.")]
        ServerRestartedWithErrorsInDynamicCodeFixErrorsAndRestartAgain,

        [Description("Remove Save Operation?")]
        RemoveSaveOperation,
    }

    public class DynamicTypeDefinition
    {
        [JsonProperty(PropertyName = "baseType")]
        public DynamicBaseType BaseType;

        [JsonProperty(PropertyName = "entityKind", NullValueHandling = NullValueHandling.Ignore)]
        public EntityKind? EntityKind;

        [JsonProperty(PropertyName = "entityData", NullValueHandling = NullValueHandling.Ignore)]
        public EntityData? EntityData;

        [JsonProperty(PropertyName = "properties")]
        public List<DynamicProperty> Properties;

        [JsonProperty(PropertyName = "operationCreate")]
        public OperationConstruct OperationCreate;

        [JsonProperty(PropertyName = "operationSave")]
        public OperationExecute OperationSave;

        [JsonProperty(PropertyName = "operationDelete")]
        public OperationDelete OperationDelete;

        [JsonProperty(PropertyName = "queryFields")]
        public List<string> QueryFields;

        [JsonProperty(PropertyName = "multiColumnUniqueIndex")]
        public MultiColumnUniqueIndex MultiColumnUniqueIndex;
        
        [JsonProperty(PropertyName = "toStringExpression", NullValueHandling = NullValueHandling.Ignore)]
        public string ToStringExpression;
    }

    public class MultiColumnUniqueIndex
    {
        [JsonProperty(PropertyName = "fields")]
        public List<string> Fields;

        [JsonProperty(PropertyName = "where")]
        public string Where;

    }

    public class OperationConstruct
    {
        [JsonProperty(PropertyName = "construct")]
        public string Construct;
    }

    public class OperationExecute
    {
        [JsonProperty(PropertyName = "canExecute")]
        public string CanExecute;

        [JsonProperty(PropertyName = "execute")]
        public string Execute;
    }

    public class OperationDelete
    {
        [JsonProperty(PropertyName = "canDelete")]
        public string CanDelete;

        [JsonProperty(PropertyName = "delete")]
        public string Delete;
    }

    public enum DynamicBaseType
    {
        Entity,
    }

    public class DynamicProperty
    {
        [JsonProperty(PropertyName = "uid")]
        public string UID;

        [JsonProperty(PropertyName = "name")]
        public string Name;

        [JsonProperty(PropertyName = "type")]
        public string Type;

        [JsonProperty(PropertyName = "isNullable")]
        public IsNullable IsNullable;

        [JsonProperty(PropertyName = "uniqueIndex")]
        public UniqueIndex UniqueIndex;

        [JsonProperty(PropertyName = "isLite", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsLite;

        [JsonProperty(PropertyName = "isMList", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsMList;

        [JsonProperty(PropertyName = "preserveOrder", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool PreserveOrder;

        [JsonProperty(PropertyName = "size", NullValueHandling = NullValueHandling.Ignore)]
        public int? Size;

        [JsonProperty(PropertyName = "scale", NullValueHandling = NullValueHandling.Ignore)]
        public int? Scale;
        
        [JsonProperty(PropertyName = "validators", NullValueHandling = NullValueHandling.Ignore)]
        public List<DynamicValidator> Validators; 
    }


    public enum IsNullable
    {
        Yes,
        OnlyInMemory,
        No,
    }

    public enum UniqueIndex
    {
        No,
        Yes,
        YesAllowNull,
    }


    class DynamicValidatorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DynamicValidator));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            var type = DynamicValidator.GetDynamicValidatorType(obj.Property("type").Value.Value<string>());

            object target = Activator.CreateInstance(type);
            serializer.Populate(obj.CreateReader(), target);
            return target;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    [JsonConverter(typeof(DynamicValidatorConverter))]
    public class DynamicValidator
    {
        [JsonProperty(PropertyName = "type")]
        public string Type;
    
        public static Type GetDynamicValidatorType(string type)
        {
            switch (type)
            {
                case "StringLength": return typeof(StringLength);
                case "Decimals": return typeof(Decimals);
                case "NumberIs": return typeof(NumberIs);
                case "CountIs": return typeof(CountIs);
                case "NumberBetween": return typeof(NumberBetween);
                case "DateTimePrecision": return typeof(DateTimePrecision);
                case "TimeSpanPrecision": return typeof(TimeSpanPrecision);
                case "StringCase": return typeof(StringCase);
                default: return typeof(DynamicValidator);
            }
        }

        public virtual string ExtraArguments()
        {
            return null;
        }

        string Value(object obj)
        {
            return CSharpRenderer.Value(obj, obj.GetType(), null);
        }


        public class StringLength : DynamicValidator
        {
            [JsonProperty(PropertyName = "allowNulls", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool AllowNulls;

            [JsonProperty(PropertyName = "multiLine", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool MultiLine;

            [JsonProperty(PropertyName = "min", NullValueHandling = NullValueHandling.Ignore)]
            public int? Min;

            [JsonProperty(PropertyName = "max", NullValueHandling = NullValueHandling.Ignore)]
            public int? Max;

            [JsonProperty(PropertyName = "allowLeadingSpaces", NullValueHandling = NullValueHandling.Ignore)]
            public bool? AllowLeadingSpaces;

            [JsonProperty(PropertyName = "allowTrailingSpaces", NullValueHandling = NullValueHandling.Ignore)]
            public bool? AllowTrailingSpaces;

            public override string ExtraArguments()
            {
                return new string[] {
                    "AllowNulls="+ Value(this.AllowNulls),
                    MultiLine ? "MultiLine=true" : null,
                    Min.HasValue ? "Min=" + Value(Min.Value) : null,
                    Max.HasValue ? "Max=" + Value(Max.Value) : null,
                    AllowLeadingSpaces.HasValue ? "AllowLeadingSpaces=" + Value(AllowLeadingSpaces.Value) : null,
                    AllowTrailingSpaces.HasValue ? "AllowTrailingSpaces=" + Value(AllowTrailingSpaces.Value) : null,
                }.NotNull().ToString(", ");
            }
        }

        public class Decimals : DynamicValidator
        {
            [JsonProperty(PropertyName = "decimalPlaces")]
            public int DecimalPlaces;

            public override string ExtraArguments()
            {
                return Value(DecimalPlaces);
            }
        }

        public class NumberIs : DynamicValidator
        {
            [JsonProperty(PropertyName = "comparisonType")]
            public ComparisonType ComparisonType;

            [JsonProperty(PropertyName = "number")]
            public decimal Number;

            public override string ExtraArguments()
            {
                return Value(ComparisonType) + ", " + Value(Number);
            }
        }

        public class CountIs : DynamicValidator
        {
            [JsonProperty(PropertyName = "comparisonType")]
            public ComparisonType ComparisonType;

            [JsonProperty(PropertyName = "number")]
            public decimal Number;

            public override string ExtraArguments()
            {
                return Value(ComparisonType) + ", " + Value(Number);
            }
        }

        public class NumberBetween : DynamicValidator
        {
            [JsonProperty(PropertyName = "min")]
            public decimal Min;

            [JsonProperty(PropertyName = "max")]
            public decimal Max;

            public override string ExtraArguments()
            {
                return Value(Min) + ", " + Value(Max);
            }
        }

        public class DateTimePrecision : DynamicValidator
        {
            [JsonProperty(PropertyName = "precision")]
            public Signum.Utilities.DateTimePrecision Precision;

            public override string ExtraArguments()
            {
                return Value(Precision);
            }
        }

        public class TimeSpanPrecision : DynamicValidator
        {
            [JsonProperty(PropertyName = "precision")]
            public Signum.Utilities.DateTimePrecision Precision;

            public override string ExtraArguments()
            {
                return Value(Precision);
            }
        }

        public class StringCase : DynamicValidator
        {
            [JsonProperty(PropertyName = "textCase")]
            public Signum.Entities.StringCase TextCase;

            public override string ExtraArguments()
            {
                return Value(TextCase);
            }
        }
    }
}