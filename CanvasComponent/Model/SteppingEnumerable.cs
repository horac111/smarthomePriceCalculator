using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CanvasComponent.Model
{
    public class SteppingEnumerable<T> : IEnumerable<T>, IEnumerable
    {
        private IEnumerable<IEnumerable<T>> list = new IEnumerable<T>[0];
        private int index = 0;
        private bool hadSeed = false;
        public IEnumerator<T> GetEnumerator()
        {
            foreach(T obj in list.Take(index).SelectMany(x => x))
                yield return obj;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void StepForward()
        {
            if (CanStepForward())
                index++;
        }

        public void StepBackward()
        {
            if(CanStepBackward())
                index--;
        }

        public void Step(bool forward)
        {
            if (forward)
                StepForward();
            else 
                StepBackward();
        }

        public bool CanStepForward()
            => index < list.Count();

        public bool CanStepBackward()
            => index > (hadSeed ? 1 : 0);

        public void AddRange(IEnumerable<T> values)
        {
            list = list.Take(index).Append(values).ToArray();
            index++;
        }

        public SteppingEnumerable(IEnumerable<T> initial)
        {
            list = new[] { initial };
            index = 1;
            hadSeed = true;
        }
        public SteppingEnumerable()
        {
        }
    }
}
