using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Entities
{
    public class Signer
    {
        private String nif;

        public String Nif
        {
            get { return nif; }
            set { nif = value; }
        }
        private String nombre;

        public String Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private String telefono;

        public String Telefono
        {
            get { return telefono; }
            set { telefono = value; }
        }
        private int page;

        public int Page
        {
            get { return page; }
            set { page = value; }
        }
        private int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        private int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        private bool signed;

        public bool Signed
        {
            get { return signed; }
            set { signed = value; }
        }
    }
}
