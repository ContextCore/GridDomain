using System;

namespace GridDomain.Scheduling.Integration
{
    public static class TaskRouterFactory
    {
        private static ITaskRouter _router;

        public static void Init(ITaskRouter router)
        {
            _router = router;
        }

        public static ITaskRouter Get()
        {
            if (_router == null)
            {
                throw new InvalidOperationException("Please initialize factory on application start");
            }
            return _router;
        }
    }
}