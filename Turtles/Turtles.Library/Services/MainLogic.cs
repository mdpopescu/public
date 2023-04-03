using System.Drawing;
using Turtles.Library.Contracts;
using Turtles.Library.Models;

namespace Turtles.Library.Services;

public class MainLogic
{
    public MainLogic(IMainForm form)
    {
        this.form = form;
    }

    public void Reset()
    {
        turtle = new Turtle(new PointF(), 0);

        // TODO
    }

    public void HelpAbout()
    {
        //
    }

    //

    private readonly IMainForm form;

    private Turtle turtle = new(new PointF(), 0);
}