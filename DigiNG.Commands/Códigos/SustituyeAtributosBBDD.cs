﻿using Digi21.Digi3D;
using Digi21.DigiNG.Entities;
using Digi21.DigiNG.Plugin.Command;
using Digi21.DigiNG.Plugin.Menu;
using Digi21.Math;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DigiNG.Commands.Códigos
{
    /// <summary>
    /// Sustituye en la segunda geometría seleccionada el valor de los atributos indicados por parámetro con los valores de la primera geometría seleccionada.
    /// </summary>
    [LocalizableCommand(typeof(Recursos), "SustituirAtributosBBDDName")]
    [LocalizableCommandInMenu(typeof(Recursos), "SustituirAtributosBBDDTitle", MenuItemGroup.EditGroup6)]
    public class SustituyeAtributosBBDD : Command
    {
        private Entity entidadOrigen;

        public SustituyeAtributosBBDD()
        {
            Initialize += CopiarAtributosBBDD_Initialize;
            SetFocus += CopiarAtributosBBDD_SetFocus;
            DataUp += CopiarAtributosBBDD_DataUp;
            EntitySelected += CopiarAtributosBBDD_EntitySelected;
        }

        private void CopiarAtributosBBDD_EntitySelected(object sender, EntitySelectedEventArgs e)
        {
            if (e.Entity.Codes.Count > 1)
            {
                Digi3D.Music(MusicType.Error);
                MessageBox.Show(Recursos.HasSeleccionadoUnaEntidadConMasDeUnCodigoEstaOrdenNoEstaPreparada);
                SolicitaSeleccionarEntidad();
                return;
            }

            if (null == entidadOrigen)
            {
                entidadOrigen = e.Entity;
                SolicitaSeleccionarEntidad();
                return;
            }

            // Copiamos los atributos de los códigos coincidentes
            if (entidadOrigen.Codes[0].Name != e.Entity.Codes[0].Name)
            {
                Digi3D.ShowBallon(
                    Recursos.SustituirAtributosBBDDName,
                    Recursos.LaEntidadOrigenYDestinoNoTienenCódigosComunes,
                    1000);
                Digi3D.Music(MusicType.Error);
                return;
            }

            var entidadClonada = e.Entity.Clone();

            for (var i = 0; i < entidadClonada.Codes.Count; i++)
            {
                entidadClonada.Codes[i] = new Code(entidadOrigen.Codes[i].Name);
                foreach(var tupla in entidadOrigen.Codes[i].Attributes)
                    entidadClonada.Codes[i].Attributes[tupla.Key] = tupla.Value;
            }

            Digi21.DigiNG.DigiNG.DrawingFile.Add(entidadClonada);
            Digi21.DigiNG.DigiNG.DrawingFile.Delete(e.Entity);
            Digi3D.Music(MusicType.EndOfLongProcess);

            entidadOrigen = null;
            SolicitaSeleccionarEntidad();
        }

        private void CopiarAtributosBBDD_DataUp(object sender, Point3DEventArgs e)
        {
            // Aquí ordenamos a DigiNG que localice entidades en las coordenadas en las que el usuario ha hecho clic con el ratón.

            // Tenemos dos casos: Si estamos solicitando al usuario la entidad origen permitimos seleccionar todas las entidades posibles, pero si estamos seleccionando el destino
            // únicamente si la entidad pertenece al archivo de dibujo (y no a los archivos de referencia). Eso se resuelve con una expresión lambda que devuelve verdadero únicamente
            // si la entidad indicada pertenece al archivo de dibujo.
            if (entidadOrigen == null)
                Digi21.DigiNG.DigiNG.SelectEntity(e.Coordinates);
            else
                Digi21.DigiNG.DigiNG.SelectEntity(e.Coordinates, entidad => Digi21.DigiNG.DigiNG.DrawingFile.Contains(entidad));
        }

        private void CopiarAtributosBBDD_SetFocus(object sender, EventArgs e)
        {
            SolicitaSeleccionarEntidad();
        }

        private void CopiarAtributosBBDD_Initialize(object sender, EventArgs e)
        {
            if (Args.Length == 0)
            {
                Digi3D.Music(MusicType.Error);
                MessageBox.Show(Recursos.DebesEspecificarElNombreDeLosCamposASustituir);

                Dispose();
                return;
            }
            SolicitaSeleccionarEntidad();
        }

        private void SolicitaSeleccionarEntidad()
        {
            Digi3D.StatusBar.Text = entidadOrigen == null ? Recursos.SeleccionaLaEntidadOrigen : Recursos.SeleccionaLaEntidadDestino;
        }
    }
}
