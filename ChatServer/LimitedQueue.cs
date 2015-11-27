using System.Collections;
using System.Collections.Generic;

namespace ChatServer {
    class LimitedQueue<T> : IEnumerable<T> {
        private readonly Queue<T> _queue;
        private readonly int _limit;

        public LimitedQueue(int limit) {
            _queue = new Queue<T>();
            _limit = limit;

        }  
        public void Enqueue(T obj) {
            if (_queue.Count == _limit) {
                _queue.Dequeue();
            } 
            _queue.Enqueue(obj);
        }

        public IEnumerator<T> GetEnumerator() {
           return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _queue.GetEnumerator();
        }
    }
}
