﻿

using System.Collections.Generic;
using System.Linq;
using static ImageSorter.Models.Enums;

namespace ImageSorter.Models;

public class ImgOrderOption
{
    public string OptionText { get; }
    public ImgOrder OptionEnum { get; }


    public ImgOrderOption(string OptionText, ImgOrder ImgOrder)
    {
        this.OptionText = OptionText;
        this.OptionEnum = ImgOrder;
    }
}

public class ImgOrderOptions
{
    public List<ImgOrderOption> Options { get; }

    public ImgOrderOption GetOrderOption(ImgOrder ImgOrder)
    {
        return this.Options.First(x => x.OptionEnum == ImgOrder);
    }

    public bool TryGetOrderOption(ImgOrder ImgOrder, out ImgOrderOption ImgOrderOption)
    {
        ImgOrderOption = this.Options.FirstOrDefault(x => x.OptionEnum == ImgOrder);
        return ImgOrderOption != null;
    }

    public ImgOrderOptions()
    {
        this.Options = new List<ImgOrderOption>()
        {
            new ImgOrderOption("File Name Ascending", ImgOrder.AscFileName),
            new ImgOrderOption("File Name Descending", ImgOrder.DescFileName),
            new ImgOrderOption("Size Ascending", ImgOrder.AscFileSize),
            new ImgOrderOption("Size Descending", ImgOrder.DescFileSize),
            new ImgOrderOption("Creation Time Ascending", ImgOrder.AscFileCreatedTime),
            new ImgOrderOption("Creation Time Descending", ImgOrder.DescFileCreatedTime),
            new ImgOrderOption("Last Modified Time Ascending", ImgOrder.AscLastModifiedTime),
            new ImgOrderOption("Last Modified Time Descending", ImgOrder.DescLastModifiedTime)
        };
    }
}