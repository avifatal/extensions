﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Operations;
using Signum.Utilities;

namespace Signum.Entities.Processes
{
    [Serializable]
    public class PackageDN : IdentifiableEntity, IProcessDataDN
    {
        [SqlDbType(Size = 200)]
        string name;
        [StringLengthValidator(AllowNulls = true , Max = 200)]
        public string Name
        {
            get { return name; }
            set { SetToStr(ref name, value, () => Name); }
        }

        OperationDN operation;
        public OperationDN Operation
        {
            get { return operation; }
            set { SetToStr(ref operation, value, () => Operation); }
        }

        int numLines;
        public int NumLines
        {
            get { return numLines; }
            set { SetToStr(ref numLines, value, () => NumLines); }
        }

        int numErrors;
        public int NumErrors
        {
            get { return numErrors; }
            set { SetToStr(ref numErrors, value, () => NumErrors); }
        }

        public override string ToString()
        {
            return "{0} {1} ({2} lines{3})".Formato(Operation, Name, numLines, numErrors == 0 ? "" : ", {0} errors".Formato(numErrors));
        }
    }

    [Serializable]
    public class PackageLineDN : IdentifiableEntity
    {
        Lite<PackageDN> package;
        public Lite<PackageDN> Package
        {
            get { return package; }
            set { Set(ref package, value, () => Package); }
        }

        [ImplementedByAll]
        Lite<IIdentifiable> target;
        public Lite<IIdentifiable> Target
        {
            get { return target; }
            set { Set(ref target, value, () => Target); }
        }

        [ImplementedByAll]
        Lite<IIdentifiable> result;
        public Lite<IIdentifiable> Result //ConstructFrom only!
        {
            get { return result; }
            set { Set(ref result, value, () => Result); }
        }

        DateTime? finishTime;
        public DateTime? FinishTime
        {
            get { return finishTime; }
            set { Set(ref finishTime, value, () => FinishTime); }
        }


        [SqlDbType(Size = int.MaxValue)]
        string exception;
        public string Exception
        {
            get { return exception; }
            set { Set(ref exception, value, () => Exception); }
        }
    }
}
