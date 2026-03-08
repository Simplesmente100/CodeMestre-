using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CivilTopoPlugin.Models;

namespace CivilTopoPlugin.Generators;

public class TabelaAutoCADGenerator
{
    public void InserirTabela(Database db, Point3d ins, List<Vertice> vertices, Transaction tr)
    {
        var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        var ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
        var tabela = new Table { Position = ins };
        tabela.SetSize(vertices.Count + 1, 4);
        tabela.SetRowHeight(3);
        tabela.SetColumnWidth(25);
        tabela.Cells[0, 0].TextString = "ID";
        tabela.Cells[0, 1].TextString = "Este";
        tabela.Cells[0, 2].TextString = "Norte";
        tabela.Cells[0, 3].TextString = "Altitude";
        for (int i = 0; i < vertices.Count; i++)
        {
            tabela.Cells[i + 1, 0].TextString = vertices[i].Id;
            tabela.Cells[i + 1, 1].TextString = vertices[i].Este.ToString("F3");
            tabela.Cells[i + 1, 2].TextString = vertices[i].Norte.ToString("F3");
            tabela.Cells[i + 1, 3].TextString = vertices[i].Altitude.ToString("F3");
        }
        ms.AppendEntity(tabela);
        tr.AddNewlyCreatedDBObject(tabela, true);
    }
}
