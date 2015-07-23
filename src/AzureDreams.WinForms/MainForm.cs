using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AzureDreams.WinForms
{
  public partial class MainForm : Form
  {
    const int CellWidth = 32;
    const int CellHeight = 32;

    private Floor currentFloor;

    public MainForm()
    {
      InitializeComponent();
      ClientSize = new Size(800, 600);
      DoubleBuffered = true;
      currentFloor = new Floor(100, 100);
    }

    private void RenderFrame(Graphics graphics)
    {
      graphics.Clear(Color.SlateBlue);

      int minC = Math.Abs(currentFloor.Cells.Min(c => c.Column));
      int minR = Math.Abs(currentFloor.Cells.Min(c => c.Row));

      foreach (var cell in currentFloor.Cells)
      {
        var color = Color.Green;
        switch (cell.Type)
        {
          case CellType.Room: { color = Color.Yellow; break; }
          case CellType.Exit: { color = Color.Red; break; }
          case CellType.Wall: { color = Colors.Lerp(Color.Yellow, Color.Black, 0.5f); break; }
        }

        using (var brush = new SolidBrush(color))
        {
          graphics.FillRectangle(brush,
            minC + cell.Column * CellWidth,
            minR + cell.Row * CellHeight,
            CellWidth, 
            CellHeight);
        }
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      RenderFrame(e.Graphics);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      timerInvalidate.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      timerInvalidate.Stop();
      base.OnFormClosed(e);
    }

    private void timerInvalidate_Tick(object sender, EventArgs e)
    {
      Invalidate();
    }
  }
}
