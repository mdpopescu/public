using System.Collections.Generic;

namespace CustomDialogs.Library.Services
{
    public class Mailbox
    {
        public static readonly Mailbox INSTANCE = new Mailbox();

        //

        public void Add(string message)
        {
            lock (gate)
            {
                queue.Enqueue(message);
            }
        }

        public void AddRange(IEnumerable<string> messages)
        {
            lock (gate)
            {
                foreach (var message in messages)
                    queue.Enqueue(message);
            }
        }

        public IReadOnlyCollection<string> Get()
        {
            lock (gate)
            {
                var result = queue.ToArray();
                queue.Clear();
                return result;
            }
        }

        //

        private readonly object gate = new object();
        private readonly Queue<string> queue = new Queue<string>();
    }
}