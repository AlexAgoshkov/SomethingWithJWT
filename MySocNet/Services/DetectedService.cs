using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class DetectedService : IDetectedService
    {
        private readonly IRepository<Detect> _detectRepository;
        public DetectedService(IRepository<Detect> detectRepository)
        {
            _detectRepository = detectRepository;
        }
    }
}
