using System;
using System.Collections.Generic;
using System.Linq;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business.async
{
    using Unit = String;

    public abstract class PartitionPolicy
    {
        private List<Unit> units;

        public List<Unit> GetSubUnits(PersistentSupport sp)
        {
            if (executed == false)
            {
                units = Breakdown(sp);
                executed = false;
            }
            return units;
        }

        protected abstract List<Unit> Breakdown(PersistentSupport sp);

        public virtual bool IsGlobal
        {
            get { return false; }
        }

        public bool executed = false;
    }


    public class GlobalPartition : PartitionPolicy
    {

        protected override List<Unit> Breakdown(PersistentSupport sp)
        {
            return new List<Unit>();
        }

        public override bool IsGlobal
        {
            get { return true; }
        }
    }

    public class SinglePartition : PartitionPolicy
    {
        private string identifier;

        public SinglePartition(string identifier)
        {
            this.identifier = identifier;
        }

        protected override List<Unit> Breakdown(PersistentSupport sp)
        {
            List<Unit> lista = new List<Unit>();
            lista.Add(identifier);
            return lista;
        }

    }

    public class MultiplePartition : PartitionPolicy
    {
        private Delegate innerPolicy;
        private List<String> identifiers;
        public MultiplePartition(Func<String, PartitionPolicy> innerPolicy, List<String> identifiers)
        {
            this.innerPolicy = innerPolicy;
            this.identifiers = identifiers;
        }
        protected override List<String> Breakdown(PersistentSupport sp)
        {
            List<Unit> list = new List<Unit>();
            foreach (var id in identifiers)
            {
                PartitionPolicy policy = innerPolicy.DynamicInvoke(id) as PartitionPolicy;
                list.AddRange(policy.GetSubUnits(sp));
            }
            return list;
        }
    }

    public abstract class QueryPartition : PartitionPolicy
    {
        protected List<Unit> ListFromMatrix(DataMatrix matrix)
        {
            List<Unit> list = new List<Unit>();

            for (int i = 0; i < matrix.NumRows; i++)
            {
                list.Add(matrix.GetString(i, 0));
            }
            return list;
        }

        protected CriteriaSet Empty(object field)
        {
            return CriteriaSet.Or().Equal(0, field).Equal(field, null);
        }
    }

    public class EmptyPolicy : PartitionPolicy
    {
        protected override List<Unit> Breakdown(PersistentSupport sp)
        {
            return new List<Unit>();
        }
    }
}