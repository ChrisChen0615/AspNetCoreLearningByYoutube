using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.Models.DISample
{
    public class SampleService
    {
        public ISampleTransient SampleTransient { get; private set; }
        public ISampleScoped SampleScoped { get; private set; }
        public ISampleSingleton SampleSingleton { get; private set; }

        public SampleService(ISampleTransient sampleTransient, 
            ISampleScoped sampleScoped, 
            ISampleSingleton sampleSingleton)
        {
            SampleTransient = sampleTransient;
            SampleScoped = sampleScoped;
            SampleSingleton = sampleSingleton;
        }
    }
}
