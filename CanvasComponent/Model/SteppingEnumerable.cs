using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CanvasComponent.Model
{
    public class SteppingEnumerable<T> : IEnumerable<T>, IEnumerable
    {
        private IEnumerable<IEnumerable<T>> list = new IEnumerable<T>[0];
        private int index = 0;
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
            if (index < list.Count())
                index++;
        }

        public void StepBackward()
        {
            if(index > 0)
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
            => index > 0;

        public void AddRange(IEnumerable<T> values)
        {
            list = list.Take(index).Append(values).ToArray();
            index++;
        }
    }
}
