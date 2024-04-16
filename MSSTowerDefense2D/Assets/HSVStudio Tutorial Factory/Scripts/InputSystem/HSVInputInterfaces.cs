using UnityEngine;

namespace HSVStudio.Tutorial
{
    public interface IHSVPointerClick
    {
        public void OnPointerClick();
    }

    public interface IHSVPointerDown
    {
        public void OnPointerDown();
    }

    public interface IHSVPointerUp
    {
        public void OnPointerUp();
    }

    public interface IHSVPointerEnter
    {
        public void OnPointerEnter();
    }

    public interface IHSVPointerExit
    {
        public void OnPointerExit();
    }
}