using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer {
    class LimitedQueue<T> {
        private Queue<T> _queue;
        private int _limit;

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
    }
}
