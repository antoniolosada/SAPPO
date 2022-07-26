/// ISC jonathan Lucas Flores
/// E-mail: jonathaa_242@hotmail.com, jonathanlf242@gmail.com
/// San Miguel Xalepec Puebla, Mexico
/// el algoritmo esta basado en el siiguiente video: http://www.youtube.com/watch?v=6rl0ghgPfK0

using System;
using System.Collections.Generic;
using System.Text;

namespace RutamasCorta.Clases
{
    /// <summary>
    /// Clase para poder simular los puntos la cual hereda todo los metodos y propiedades de la clase Label
    /// </summary>
    class clsPunto : System .Windows .Forms .Label 
    {
        #region "variables"
        /// <summary>
        /// Variable en la cual se guardara el valor acumulado de la distancia
        /// </summary>
        private int Acumulado;
        /// <summary>
        /// Variable en la cual de guardara el punto de Procedencia
        /// </summary>
        private int procedencia;
        /// <summary>
        /// Variable n la cual se guardara el ID para identificar que punto es
        /// </summary>
        private int ID;
        #endregion
        
        #region"Propiedades"
        /// <summary>
        /// Propiedad para acceder a la variable acumulado
        /// </summary>
        public int P_acumulado
        { 
            get{return Acumulado ;}
            set{Acumulado =value;}
        }
        /// <summary>
        /// Propiedad para aceder a la variable procedencia
        /// </summary>
        public int P_procedencia
        {
            get { return procedencia ; }
            set { procedencia  = value; }
        }
        /// <summary>
        /// Propiedad para acceder a la variable ID
        /// </summary>
        public int P_id
        {
            get { return ID ; }
            set { ID  = value; }
        }
        #endregion
    }
}
