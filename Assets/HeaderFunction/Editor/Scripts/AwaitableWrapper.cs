using System.Threading.Tasks;

namespace NotBura.Packages
{
    public struct AwaitableWrapper
    {
        private AwaitableFactory m_factory;
        private object m_awaiter;

        public AwaitableWrapper(object awaiter, AwaitableFactory factory)
        {
            m_awaiter = awaiter;
            m_factory = factory;
        }

        public async ValueTask CompleteAsync()
        {
            while (false == IsCompleted())
            {
                await Task.Yield();
            }
        }

        public bool IsCompleted()
        {
            return m_factory.IsCompleted(m_awaiter);
        }

        public object GetResult()
        {
            return m_factory.GetResult(m_awaiter);
        }
    }
}
