// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
	$('#searchtextbox').focus(function () {
		$('#maincontent').find('*').not('#searchtextbox').not('#searchform').addClass('blurredElement');
	});

	$('#searchtextbox').blur(function () {
		$('#maincontent').find('*').not('#searchtextbox').not('#searchform').removeClass('blurredElement');
	});
});
