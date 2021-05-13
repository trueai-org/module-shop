using IdGen;
using System.Threading;

namespace Shop.Infrastructure
{
    public class NoGen
    {
        const int GEN_ORDER_NO_ID = 0;

        static NoGen _instance;
        static IdGenerator _genOrderNo = new IdGenerator(GEN_ORDER_NO_ID);

        NoGen() { }

        public static NoGen Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                Interlocked.CompareExchange(ref _instance, new NoGen(), null);
                return _instance;
            }
        }

        public long GenOrderNo()
        {
            return _genOrderNo.CreateId();
        }
    }
}
