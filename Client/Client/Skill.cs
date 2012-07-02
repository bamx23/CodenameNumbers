using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    public class SkillUsedEventArgs : EventArgs
    {
        public DateTime UseTime { get; protected set; }

        public SkillUsedEventArgs(DateTime useTime)
        {
            UseTime = useTime;
        }
    }

    public class Skill : INotifyPropertyChanged
    {
        private string skillName;
        private long cooldown;
        private DateTime lastUse;
        private Key useKey;

        public event EventHandler<SkillUsedEventArgs> Used;

        public string SkillName { get { return skillName; } protected set { skillName = value; NotifyPropertyChanged("SkillName"); } }
        public bool IsCooldown { get { return (DateTime.UtcNow - lastUse).TotalMilliseconds < cooldown; } }
        public long Cooldown { get { return (long)((DateTime.UtcNow - lastUse).TotalMilliseconds*100 / cooldown); } }
        public Key SkillKey { get { return useKey; } }

        /// <summary>
        /// For visualisation
        /// </summary>
        public Visibility Visible { get { return IsCooldown ? Visibility.Visible : Visibility.Hidden; } }

        public Skill(string name, Key useKey, long cooldown)
        {
            this.skillName = name;
            this.useKey = useKey;
            this.cooldown = cooldown;

            Game.timer.Elapsed += Update;
        }

        private void Update(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            NotifyPropertyChanged("Visible");
            if(IsCooldown)
                NotifyPropertyChanged("Cooldown");
        }

        /// <summary>
        /// Uses this skill if it available
        /// </summary>
        /// <returns>Skill been used</returns>
        public bool Use()
        {
            if (IsCooldown) return false;

            lastUse = DateTime.UtcNow;
            NotifyPropertyChanged("Cooldown");

            if (Used != null)
                Used(this, new SkillUsedEventArgs(lastUse));

            return true;
        }

        public void KeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == SkillKey) Use();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
