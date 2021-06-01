using System;

namespace HeadlessUI.Utilities
{
    public class SearchAssistant : IDisposable
    {
        public int DebouceTimeout { get; set; } = 350;
        public string SearchQuery { get; private set; }

        public event EventHandler OnChange;

        private System.Timers.Timer debounceTimer;
        public void Search(string key)
        {
            SearchQuery += key;
            OnChange?.Invoke(this, EventArgs.Empty);
            StartDebounceTimer();
        }
        private void DebounceElapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            ClearSearch();
            debounceTimer.Dispose();
        }
        private void StartDebounceTimer()
        {
            ClearDebounceTimer();

            debounceTimer = new System.Timers.Timer(DebouceTimeout);
            debounceTimer.Elapsed += DebounceElapsed;
            debounceTimer.Enabled = true;
        }
        private void ClearDebounceTimer()
        {
            if (debounceTimer != null)
            {
                debounceTimer.Enabled = false;
                debounceTimer.Dispose();
                debounceTimer = null;
            }
        }
        public void ClearSearch()
        {
            ClearDebounceTimer();
            SearchQuery = "";
            OnChange?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => ClearSearch();
    }
}
