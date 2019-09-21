jQuery(function($){
	$.datepicker.regional['tr'] = {
		closeText: 'kapat',
		prevText: '&#x3c;geri',
		nextText: 'ileri&#x3e',
		currentText: 'bugün',
		monthNames: ['Ocak','Subat','Mart','Nisan','Mayis','Haziran',
		'Temmuz','Agustos','Eylul','Ekim','Kasim','Aralik'],
		monthNamesShort: ['Oca','Sub','Mar','Nis','May','Haz',
		'Tem','Ağu','Eyl','Eki','Kas','Ara'],
		dayNames: ['Pazar','Pazartesi','Sali','Carsamba','Persembe','Cuma','Cumartesi'],
		dayNamesShort: ['Pz','Pt','Sa','Ca','Pe','Cu','Ct'],
		dayNamesMin: ['Pz','Pt','Sa','Ca','Pe','Cu','Ct'],
		weekHeader: 'Hf',
		dateFormat: 'dd.mm.yy',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ''};
	$.datepicker.setDefaults($.datepicker.regional['tr']);
});