﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MyDevHabit.Api.Services;

namespace MyDevHabit.Api.DTOs.Common;

public record AcceptHeaderDto
{
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }

    public bool IncludeLinks =>
        MediaTypeHeaderValue.TryParse(Accept, out MediaTypeHeaderValue? mediaType) &&
        mediaType.SubTypeWithoutSuffix.HasValue &&
        mediaType.SubTypeWithoutSuffix.Value.Contains(
            CustomMediaTypeNames.Application.HateoasSubType,
            StringComparison.InvariantCulture);
}
