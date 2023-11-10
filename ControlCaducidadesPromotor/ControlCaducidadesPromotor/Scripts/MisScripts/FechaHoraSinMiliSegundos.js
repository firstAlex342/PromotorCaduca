class FechaHoraSinMiliSegundos {
    constructor(fechaHora) {
        this.fechaHora = fechaHora;  //fechaHora es una cadena en este formato  30/09/2020 09:50:47 p.m.
    }

    GetDiaDe() {
        let c = this.fechaHora.substring(0, 2);
        return (c);
    }

    GetMesDe() {
        let c = this.fechaHora.substring(3, 5);
        return (c);
    }

    GetAnioDe() {
        let c = this.fechaHora.substring(6, 10);
        return (c);
    }

    GetHoraEnFormato24HrsDe() {
        let horaEnFormatoCadena = this.fechaHora.substring(11, 13);
        let horaEnFormatoInt = parseInt(horaEnFormatoCadena);

        let cadenaAmOPm = this.fechaHora.substring(20, 24);
        if (cadenaAmOPm == "a.m.") {
            return (horaEnFormatoCadena);
        }

        else { //la hora es p.m.
            let n = horaEnFormatoInt + 12;
            return (n.toString());
        }
    }

    GetMinutosDe() {
        let c = this.fechaHora.substring(14, 16);
        return (c);
    }

    GetSegundos() {
        let c = this.fechaHora.substring(17, 19);
        return (c);
    }

    GenerarObjetoDateSinMilisegs() {
        //regresa un objeto javascript Date, sin milisegundos en este formato, cuya inicializacion es asi 1985-09-15T23:59:59

        let dia = this.GetDiaDe();
        let mes = this.GetMesDe();
        let anio = this.GetAnioDe();
        let horaEnFormato24Hrs = this.GetHoraEnFormato24HrsDe();
        let minutos = this.GetMinutosDe();
        let segundos = this.GetSegundos();

        let fechaHoraSInMilisegs = new Date(anio + "-" + mes + "-" + dia + "T" + horaEnFormato24Hrs + ":" + minutos + ":" + segundos);
        return (fechaHoraSInMilisegs);
    }//No Usar este metodo


    FuckYou(objetoDate) {
        //¡¡¡ Regresa un objeto Date donde El dia y mes deben de estar a dos digitos  !! 
        let miObjDate = new Date(objetoDate);

        let miDiaEnTexto;
        if (objetoDate.getDate() <= 9) {
            miDiaEnTexto = "0" + miObjDate.getDate().toString();
        }

        else {
            miDiaEnTexto = miObjDate.getDate().toString();
        }


        let miMesEnTexto;
        if (miObjDate.getMonth() <= 8) {
            let mesEnNumero = parseInt(miObjDate.getMonth()) + 1;
            miMesEnTexto = "0" + mesEnNumero.toString();
        }

        else {
            let mesEnNumero = parseInt(miObjDate.getMonth()) + 1;
            miMesEnTexto = mesEnNumero.toString();
        }



        let miAnioEnTexto = miObjDate.getFullYear().toString();


        let miHoraEnFormato24hrs;
        if (miObjDate.getHours().toString().length == 1) {
            miHoraEnFormato24hrs = "0" + miObjDate.getHours().toString();
        }

        else {
            miHoraEnFormato24hrs = miObjDate.getHours().toString();
        }


        let miMinutosEnTexto;
        if (miObjDate.getMinutes().toString().length == 1) {
            miMinutosEnTexto = "0" + miObjDate.getMinutes().toString();
        }

        else {
            miMinutosEnTexto = miObjDate.getMinutes().toString();
        }


        let miSegundosEnTexto;
        if (miObjDate.getSeconds().toString().length == 1) {
            miSegundosEnTexto = "0" + miObjDate.getSeconds().toString();
        }

        else {
            miSegundosEnTexto = miObjDate.getSeconds().toString();
        }


        let objetoDateSinMiliSegs = new Date(miAnioEnTexto + "-" + miMesEnTexto + "-" + miDiaEnTexto + "T" + miHoraEnFormato24hrs + ":" + miMinutosEnTexto + ":" + miSegundosEnTexto);
        return (objetoDateSinMiliSegs);
    }

    GenerarEnSintaxisCadena() {
        //Genera una cadena por ejemplo: 2015-05-02T23:50:59 , esta cadena puede ser 
        //usada para enviar una fecha / hora a un controller API
        let dia = this.GetDiaDe();
        let mes = this.GetMesDe();
        let anio = this.GetAnioDe();
        let horaEnFormato24Hrs = this.GetHoraEnFormato24HrsDe();
        let minutos = this.GetMinutosDe();
        let segundos = this.GetSegundos();

        let cadena = anio + "-" + mes + "-" + dia + "T" + horaEnFormato24Hrs + ":" + minutos + ":" + segundos;
        return (cadena);
    }

    GenerarObjetoDateDeFormato(texto) {
        // texto es una cadena que esta en este formato: Sun Sep 15 1985 23:59:59 GMT-0500 (hora estándar oriental)
        //texto es una cadena que se extrae de un instrucción como esta:
        // document.getElementById('miTablaProductosEnTienda').rows[i].cells[2].childNodes[0].nodeValue

        let dateAlmacena_FechaAlta = new Date(texto);
        let objDateAlmacena_FechaAlta = this.FuckYou(dateAlmacena_FechaAlta);
        return (objDateAlmacena_FechaAlta);
    }
}

/*  Lo de abajo es la clase de arriba, pero usando "var" y el operador function debe su util para versiones anteriores a ES6
function FechaHoraSinMiliSegundos(fechaHora) {
    this.fechaHora = fechaHora;  //fechaHora es una cadena en este formato  30/09/2020 09:50:47 p.m.

    this.GetDiaDe = GetDiaDe;
    this.GetMesDe = GetMesDe;
    this.GetAnioDe = GetAnioDe;
    this.GetHoraEnFormato24HrsDe = GetHoraEnFormato24HrsDe;
    this.GetMinutosDe = GetMinutosDe;
    this.GetSegundos = GetSegundos;

    this.GenerarObjetoDateSinMilisegs = GenerarObjetoDateSinMilisegs;
    this.FuckYou = FuckYou;
    this.GenerarEnSintaxisCadena = GenerarEnSintaxisCadena;
    this.GenerarObjetoDateDeFormato = GenerarObjetoDateDeFormato;
}


function GetDiaDe() {
    var c = this.fechaHora.substring(0, 2);
    return (c);
}

function GetMesDe() {
    var c = this.fechaHora.substring(3, 5);
    return (c);
}

function GetAnioDe() {
    var c = this.fechaHora.substring(6, 10);
    return (c);
}

function GetHoraEnFormato24HrsDe() {
      var horaEnFormatoCadena = this.fechaHora.substring(11, 13);   
      var horaEnFormatoInt = parseInt(horaEnFormatoCadena);


      var cadenaAmOPm = this.fechaHora.substring(20, 24);
      if (cadenaAmOPm == "a.m.") {
          return (horaEnFormatoCadena);
       }

      else { //la hora es p.m.
        var n = horaEnFormatoInt + 12;
        return (n.toString());
      }
}

function GetMinutosDe() {
    var c = this.fechaHora.substring(14, 16);
    return (c);
}

function GetSegundos() {
    var c = this.fechaHora.substring(17, 19);
    return (c);
}


function GenerarObjetoDateSinMilisegs() {        
    //regresa un objeto javascript Date, sin milisegundos en este formato, cuya inicializacion es asi 1985-09-15T23:59:59

    var dia = this.GetDiaDe();
    var mes = this.GetMesDe();
    var anio = this.GetAnioDe();
    var horaEnFormato24Hrs = this.GetHoraEnFormato24HrsDe();
    var minutos = this.GetMinutosDe();
    var segundos = this.GetSegundos();

    var fechaHoraSInMilisegs = new Date(anio + "-" + mes + "-" + dia + "T" + horaEnFormato24Hrs + ":" + minutos + ":" + segundos);
    return (fechaHoraSInMilisegs);
}//No Usar este metodo



function FuckYou(objetoDate) {
    //¡¡¡ Regresa un objeto Date donde El dia y mes deben de estar a dos digitos  !! 
    var miObjDate = new Date(objetoDate);

    var miDiaEnTexto;
    if (objetoDate.getDate() <= 9) {
         miDiaEnTexto = "0" + miObjDate.getDate().toString();
    }

    else {
        miDiaEnTexto = miObjDate.getDate().toString();
    }


    var miMesEnTexto;
    if (miObjDate.getMonth() <= 8) {
        var mesEnNumero = parseInt(miObjDate.getMonth()) + 1;
        miMesEnTexto = "0" + mesEnNumero.toString();
    }

    else {
        var mesEnNumero = parseInt(miObjDate.getMonth()) + 1;
        miMesEnTexto = mesEnNumero.toString();
    }



    var miAnioEnTexto = miObjDate.getFullYear().toString();
    

    var miHoraEnFormato24hrs;
    if (miObjDate.getHours().toString().length == 1) {
        miHoraEnFormato24hrs = "0" + miObjDate.getHours().toString();
    }

    else {
        miHoraEnFormato24hrs = miObjDate.getHours().toString();
    }


    var miMinutosEnTexto;
    if (miObjDate.getMinutes().toString().length == 1) {
        miMinutosEnTexto = "0" + miObjDate.getMinutes().toString();
    }

    else {
        miMinutosEnTexto = miObjDate.getMinutes().toString();
    }


    var miSegundosEnTexto;
    if (miObjDate.getSeconds().toString().length == 1) {
        miSegundosEnTexto = "0" + miObjDate.getSeconds().toString();
    }

    else {
        miSegundosEnTexto = miObjDate.getSeconds().toString();
    }


    var objetoDateSinMiliSegs = new Date(miAnioEnTexto + "-" + miMesEnTexto + "-" + miDiaEnTexto + "T" + miHoraEnFormato24hrs + ":" + miMinutosEnTexto + ":" + miSegundosEnTexto);
    return (objetoDateSinMiliSegs);
}


function GenerarEnSintaxisCadena() {
    //Genera una cadena por ejemplo: 2015-05-02T23:50:59 , esta cadena puede ser 
    //usada para enviar una fecha / hora a un controller API
    var dia = this.GetDiaDe();
    var mes = this.GetMesDe();
    var anio = this.GetAnioDe();
    var horaEnFormato24Hrs = this.GetHoraEnFormato24HrsDe();
    var minutos = this.GetMinutosDe();
    var segundos = this.GetSegundos();

    var cadena = anio + "-" + mes + "-" + dia + "T" + horaEnFormato24Hrs + ":" + minutos + ":" + segundos;
    return (cadena);
}


function GenerarObjetoDateDeFormato(texto) {
    // texto es una cadena que esta en este formato: Sun Sep 15 1985 23:59:59 GMT-0500 (hora estándar oriental)
    //texto es una cadena que se extrae de un instrucción como esta:
    // document.getElementById('miTablaProductosEnTienda').rows[i].cells[2].childNodes[0].nodeValue
    
    var dateAlmacena_FechaAlta = new Date(texto);
    var objDateAlmacena_FechaAlta = this.FuckYou(dateAlmacena_FechaAlta);
    return (objDateAlmacena_FechaAlta);
}
*/