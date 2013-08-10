using System.Collections.Generic;

namespace Renfield.VideoSpinner
{
    public interface Shuffler
    {
        IList<T> Shuffle<T>(IList<T> list);
    }
}