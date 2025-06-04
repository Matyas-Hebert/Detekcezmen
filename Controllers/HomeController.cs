using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using detekcezmen.Models;
using detekcezmen.Services;

namespace detekcezmen.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Analyzer _analyzer;

    public HomeController(ILogger<HomeController> logger, Analyzer analyzer)
    {
        _logger = logger;
        _analyzer = analyzer;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Analyze(string srcfolder)
    {
        var result = _analyzer.Analyze(srcfolder);
        return View("index", result);
    }
}
