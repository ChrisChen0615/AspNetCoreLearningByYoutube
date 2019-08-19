using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.Models.DISample
{
    public interface ISample
    {
        Guid Id { get; }
    }

    public interface ISampleTransient : ISample
    {
    }

    public interface ISampleScoped : ISample
    {
    }

    public interface ISampleSingleton : ISample
    {
    }

    public class Sample : ISampleTransient, ISampleScoped, ISampleSingleton
    {
        private Guid _id;

        public Sample()
        {
            _id = Guid.NewGuid();
        }

        public Guid Id => _id;
    }
}
