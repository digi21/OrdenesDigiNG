﻿using System;
using System.Linq;
using Digi21.Digi3D;
using Digi21.DigiNG.Entities;
using Digi21.DigiNG.Plugin.Command;
using Digi21.DigiNG.Plugin.Shell;
using Digi21.Math;

namespace DigiNG.Commands.Operaciones_con_entidades
{
    [LocalizableCommand(typeof(Recursos), "RecortarPoligonoName")]
    [LocalizableCommandInMenu(typeof(Recursos), "RecortarPoligonoTitle", MenuItemGroup.EditGroup9Group15)]
    public class RecortarPoligono : Command
    {
        private Entity entidadADividir;

        public RecortarPoligono()
        {
            Initialize += RecortarPoligono_Initialize;
            SetFocus += RecortarPoligono_SetFocus;
            DataUp += RecortarPoligono_DataUp;
            EntitySelected += RecortarPoligono_EntitySelected;
        }

        private void RecortarPoligono_SetFocus(object sender, EventArgs e)
        {
            SolicitaSeleccionaEntidad();
        }

        private void RecortarPoligono_Initialize(object sender, EventArgs e)
        {
            SolicitaSeleccionaEntidad();
        }

        private void RecortarPoligono_EntitySelected(object sender, EntitySelectedEventArgs e)
        {
            if (entidadADividir == null)
            {
                entidadADividir = e.Entity;
                SolicitaSeleccionaEntidad();
                return;
            }

            var límite = e.Entity as ReadOnlyLine;

            try
            {
                var polígonos = ((ITrimable)entidadADividir).Trim(límite);
                Digi21.DigiNG.DigiNG.DrawingFile.Add(polígonos);
                Digi21.DigiNG.DigiNG.DrawingFile.Delete(entidadADividir);

                if (Args.Length != 0 && Digi21.DigiNG.DigiNG.DrawingFile.Contains(límite))
                    Digi21.DigiNG.DigiNG.DrawingFile.Delete(límite);
            }
            catch (Exception ex)
            {
                Digi3D.Music(MusicType.Error);
                Digi3D.ShowBallon(
                    Recursos.RecortarPoligonoName,
                    ex.Message,
                    2);
            }
            finally
            {
                Dispose();
            }
        }

        private void RecortarPoligono_DataUp(object sender, Point3DEventArgs e)
        {
            if (entidadADividir == null)
                Digi21.DigiNG.DigiNG.SelectEntity(e.Coordinates, entidad => Digi21.DigiNG.DigiNG.DrawingFile.Contains(entidad) && (entidad is ReadOnlyPolygon || entidad is ReadOnlyLine line && line.Closed));
            else
                Digi21.DigiNG.DigiNG.SelectEntity(e.Coordinates, entidad => entidad is ReadOnlyLine);
        }

        private void SolicitaSeleccionaEntidad()
        {
            Digi3D.StatusBar.Text = entidadADividir == null ? Recursos.SeleccionaElPolígonoARecortar : Recursos.SeleccionaLaLíneaDeCorte;
        }

    }
}
