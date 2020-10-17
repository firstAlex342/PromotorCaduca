function FechaHoraSinMiliSegundos(fechaHora) {
    this.fechaHora = fechaHora;  //fechaHora es una cadena en este formato  30/09/2020 09:50:47 p.m.

    this.GetDiaDe = GetDiaDe;
    this.GetMesDe = GetMesDe;
    this.GetAnioDe = GetAnioDe;
    this.GetHoraEnFormato24HrsDe = GetHoraEnFormato24HrsDe;
    this.GetMinutosDe = GetMinutosDe;
    this.GetSegundos = GetSegundos;

    this.GenerarObjetoDateSinMilisegs = GenerarObjetoDateSinMilisegs;
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
    //regresa un objeto javascript Date, sin milisegundos

    var dia = this.GetDiaDe(this.fechaHora);
    var mes = this.GetMesDe(this.fechaHora);
    var anio = this.GetAnioDe(this.fechaHora);
    var horaEnFormato24Hrs = this.GetHoraEnFormato24HrsDe();
    var minutos = this.GetMinutosDe(this.fechaHora);
    var segundos = this.GetSegundos(this.fechaHora);

    var fechaHoraSInMilisegs = new Date(anio + "-" + mes + "-" + dia + "T" + horaEnFormato24Hrs + ":" + minutos + ":" + segundos);
    return (fechaHoraSInMilisegs);
}