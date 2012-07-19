using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Client
{
    public class LambdaCollection<T> : Collection<T>
        where T : DependencyObject, new()
    {
        public LambdaCollection(int count)
        {
            while(count --> 0)
                Add(new T());
        }

        public LambdaCollection<T> WithProperty<TU>(DependencyProperty property, Func<int, TU> generator)
        {
            for (int i = 0; i < Count; i++)
                Items[i].SetValue(property, generator(i));
            return this;
        }

        public LambdaCollection<T> WithXY<TU>(Func<int, TU> xGen, Func<int, TU> yGen)
        {
            for (int i = 0; i < Count; i++)
            {
                Items[i].SetValue(Canvas.LeftProperty, xGen(i));
                Items[i].SetValue(Canvas.TopProperty, yGen(i));
            }
            return this;
        }
    }

    public class LambdaDoubleAnimation : DoubleAnimation
    {
        public Func<double, double> ValueGenerator;

        private LambdaDoubleAnimation coreInstance;

        protected override Freezable CreateInstanceCore()
        {
            return coreInstance ?? (coreInstance = new LambdaDoubleAnimation
                                                       {
                                                           ValueGenerator = ValueGenerator
                                                       });
        }

        protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock)
        {
            var value = base.GetCurrentValueCore(defaultOriginValue, defaultDestinationValue, animationClock);
            if (ValueGenerator != null)
            {
                return ValueGenerator(value);
            }
            return value;
        }
    }

    public class LambdaDoubleAnimationCollection : Collection<LambdaDoubleAnimation>
    {
        public LambdaDoubleAnimationCollection(int count,
            Func<int, double> from, Func<int, double> to,
            Func<int, Duration> duration,
            Func<int,Func<double,double>> valueGenerator)
        {
            for (int i = 0; i < count; i++)
            {
                var lda = new LambdaDoubleAnimation
                              {
                                  From = from(i),
                                  To = to(i),
                                  Duration = duration(i),
                                  ValueGenerator = valueGenerator(i),
                                  RepeatBehavior = RepeatBehavior.Forever
                              };

                Add(lda);
            }
        }

        public void BeginApplyAnimation(UIElement[] targets, DependencyProperty property)
        {
            for (int i = 0; i < Count; i++)
            {
                targets[i].BeginAnimation(property, Items[i]);
            }
        }
    }

    public interface IAnimation
    {
        void Play();
        void Stop();
        bool Playing { get; set; }
    }

    public class LoadingAnimation : IAnimation
    {
        public LambdaCollection<Ellipse> Circles;
        public LambdaDoubleAnimationCollection AnimationX;
        public LambdaDoubleAnimationCollection AnimationY;

        protected bool started;
        protected bool animated;
        protected Canvas canvas;

        public LoadingAnimation(Canvas canvas, int count = 10, double radius = 10, double seconds = 1)
        {
            Circles = new LambdaCollection<Ellipse>(count)
                .WithProperty(Shape.FillProperty,
                              i => new SolidColorBrush(Color.FromArgb((byte) (255/count*(i+1)),0, 0, 0)))
                .WithProperty(FrameworkElement.WidthProperty, i => radius / 2)
                .WithProperty(FrameworkElement.HeightProperty, i => radius / 2);

            var duration = new Duration(TimeSpan.FromSeconds(seconds));

            AnimationX = new LambdaDoubleAnimationCollection(count,
                                                             f => f * 2 * Math.PI / count,
                                                             t => t * 2 * Math.PI / count + Math.PI * 2,
                                                             d => duration,
                                                             i => t => canvas.Width * 0.5 + radius * (1.1 * Math.Cos(t)));

            AnimationY = new LambdaDoubleAnimationCollection(count,
                                                             f => f * 2 * Math.PI / count,
                                                             t => t * 2 * Math.PI / count + Math.PI * 2,
                                                             i => duration,
                                                             i => t => canvas.Height * 0.4 + radius * (Math.Sin(-t) * Math.Cos(t)));

            animated = false;
            started = false;
            this.canvas = canvas;
        }

        public void Play()
        {
            if (!started)
            {
                foreach (var e in Circles)
                    canvas.Children.Add(e);

                if (!animated)
                {
                    AnimationX.BeginApplyAnimation(Circles.Cast<UIElement>().ToArray(), Canvas.LeftProperty);
                    AnimationY.BeginApplyAnimation(Circles.Cast<UIElement>().ToArray(), Canvas.TopProperty);
                    animated = true;
                }
                started = true;
            }
        }

        public void Stop()
        {
            if (!started) return;
            
            foreach (var e in Circles)
                canvas.Children.Remove(e);

            started = false;
        }

        public bool Playing
        {
            get { return started; }
            set
            {
                if (value != started)
                    if (value)
                        Play();
                    else
                        Stop();
            }
        }
    }
}
