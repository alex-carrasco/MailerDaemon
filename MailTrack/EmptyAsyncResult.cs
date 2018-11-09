using System;
using System.Threading;

namespace MailTrack
{
    public class EmptyAsyncResult : IAsyncResult
    {
        private readonly AsyncCallback m_cb = null;

        public bool IsCompleted { get; private set; } = false;

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }

        public object AsyncState { get; } = null;

        public bool CompletedSynchronously { get; private set; } = false;

        public EmptyAsyncResult(ref object state)
        {
            AsyncState = state;
            m_cb = null;
            IsCompleted = false;
        }

        public EmptyAsyncResult(ref AsyncCallback cb, ref object state)
        {
            AsyncState = state;
            m_cb = cb;
            IsCompleted = false;
        }

        public EmptyAsyncResult(ref AsyncCallback cb, ref object state, bool CompletedSynchronously, bool Completed)
        {
            AsyncState = state;
            m_cb = cb;
            if (CompletedSynchronously == true)
                SetCompletedSynchronously();
            if (IsCompleted == true)
                SetCompleted();
            else
                IsCompleted = false;
        }

        public void SetCompleted()
        {
            IsCompleted = true;
            m_cb?.Invoke(this);
        }

        public void SetCompletedSynchronously()
        {
            CompletedSynchronously = true;
        }
    }
}