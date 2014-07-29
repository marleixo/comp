using System;
using System.Collections.Generic;

namespace DMExport.Library
{
    public delegate void ProgreessChanged(Progress progress);
    
    #region Inner Classes

    /// <summary>
    /// Progress Entity
    /// </summary>
    public class Progress
    {
        /// <summary>
        /// Percent value
        /// </summary>
        public int Percent { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        public Progress(int percent, string message)
        {
            Percent = percent;
            Message = message;
        }
    }

    /// <summary>
    /// Progress Range Entity
    /// </summary>
    public class ProgressRange
    {
        /// <summary>
        /// Start
        /// </summary>
        public int Start { get; set; }
        
        /// <summary>
        /// End
        /// </summary>
        public int End { get; set; }

        public ProgressRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Range
        {
            get { return End - Start; }
        }
    }

    #endregion
    
    public class ProgressBarReporter
    {
        /// <summary>
        /// Specify method to call when progress is updated.
        /// </summary>
        public event ProgreessChanged OnProgressChanged;

        /// <summary>
        /// Values range
        /// </summary>
        public ProgressRange Range { get; set; }

        /// <summary>
        /// Storage with messages
        /// </summary>
        public IList<string> Data { get; set; }

        private int _current = -1;
        
        public ProgressBarReporter(IList<string> messageStorage, int rangeStart, int rangeEnd)
        {
            Data = messageStorage;
            Range = new ProgressRange(rangeStart, rangeEnd);
        }
        
        public void ProgressUpdateProgress()
        {
            if (OnProgressChanged != null)
            {
                OnProgressChanged(UpdateProgress(++_current));
            }
        }

        /// <summary>
        /// Updates progress.
        /// </summary>
        /// <param name="value">Step value</param>
        /// <returns>Progress instance</returns>
        private Progress UpdateProgress(int value)
        {
            var message = Data[value];
            var percent = Convert.ToInt32(
                Math.Round((double)(value * Range.Range / (Data.Count - 1)))) + Range.Start;

            return new Progress(percent, message);
        }
    }
}
