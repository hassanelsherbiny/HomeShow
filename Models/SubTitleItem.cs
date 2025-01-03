using System;
namespace HomeShow.Models
{
    public class SubTitleItem
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Text { get; set; }
        public string ToStr()
        {
            return string.Format("{0:00}:{1:00}:{2:00},{3:000} --> {4:00}:{5:00}:{6:00},{7:000}{8}{9}{8}{8}",
                        Start.Hours,
                        Start.Minutes,
                        Start.Seconds,
                        Start.Milliseconds,
                        End.Hours,
                        End.Minutes,
                        End.Seconds,
                        End.Milliseconds,
                        Environment.NewLine,
                        Text
                        );
        }

        public string ToVttLine()
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000} --> {4:00}:{5:00}:{6:00}.{7:000}{8}{9}{8}{8}",
                        Start.Hours,
                        Start.Minutes,
                        Start.Seconds,
                        Start.Milliseconds,
                        End.Hours,
                        End.Minutes,
                        End.Seconds,
                        End.Milliseconds,
                        Environment.NewLine,
                        Text
                        );
        }

        public void Shift(TimeSpan ts)
        {
            Start = Start.Add(ts);
            End = End.Add(ts);
        }
    }
}