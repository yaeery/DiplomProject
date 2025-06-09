using System;
using System.Collections.Generic;
using System.Text;

namespace Diplom2
{
    public class ImageClassificationModel
    {
        public ImageClassificationModel(string tagName, float probability)
        {
            TagName = tagName;
            Probability = probability;
        }

        public float Probability { get; }
        public string TagName { get; }
    }
}
