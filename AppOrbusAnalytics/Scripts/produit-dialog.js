

$('#btnProdValider1').click(function () {
    var codesProduit = [];
    $('#produitTable tbody input[type="checkbox"]:checked').each(function () {       
        var codeProduit = $(this).val();
        codesProduit.push(codeProduit);
      
    });
    
    $('#Produit').val(codesProduit.join(', '));
});



$('#btnProdCocherTout').click(function (e) {
    e.stopPropagation(); 
    $('#produitTable tbody input[type="checkbox"]').prop('checked', true);
});

$('#btnProdEffacer').click(function () {
    $('#txtCodeProduit').val(''); 
    $('#txtDesignation').val(''); 
    $('#produitTable tbody').empty(); 

    $('#produit-dialog').modal('hide'); 
});





function chargerListeProduit(codeProduit, designation) {
    

    $.ajax({       
        url: '/Statistique/ChargerListeProduit',
        type: 'POST',
        data: { codeProduit: codeProduit, designation: designation },
        success: function (data) {

           
            var tbody = $('#produitTable tbody');
            tbody.empty();
            for (var i = 0; i < data.length; i++) {
                var row = '<tr>' +
                    '<td><input type="checkbox" value="' + data[i].codeProduit + '" title="Choisir" /></td>' +
                    '<td>' + data[i].codeProduit + '</td>' +
                    '<td>' + data[i].designation + '</td>' +
                    '</tr>';
                tbody.append(row);
            }


         
        },
        error: function (xhr, status, error) {
            // Gérez les erreurs éventuelles lors de l'appel AJAX.
        }
    });
}



$('#txtCodeProduit').on('input', function () {
  
    var codeProduit = $('#txtCodeProduit').val();
    var designation = $('#txtDesignation').val();

    chargerListeProduit(codeProduit, designation);
});

$('#txtDesignation').on('input', function () {

    var codeProduit = $('#txtCodeProduit').val();
    var designation = $('#txtDesignation').val();

    chargerListeProduit(codeProduit, designation);
});


