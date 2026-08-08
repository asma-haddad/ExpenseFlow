param(
    [string]$Root = (Get-Location).Path,
    [string]$ProjectName = "ExpenseFlow"
)

$ErrorActionPreference = "Stop"

$projects = @{
    "$ProjectName.Api" = @(
        "Authorization\",
        "Controllers\Dashboard\",
        "Controllers\Help\",
        "Controllers\Report\",
        "Extension\",
        "Middleware\",
        "logs\",
        "Properties\PublishProfiles\",
        "wwwroot\swagger\"
    )

    "$ProjectName.Application" = @(
        "Abstraction\",
        "Extensions\",
        "Features\Dashboard\Auth\Login\",
        "Features\Dashboard\Auth\Logout\",
        "Features\Dashboard\Auth\RefreshToken\",
        "Features\Dashboard\Auth\ForgotPassword\",
        "Features\Dashboard\Auth\ResetPassword\",

        "Features\Dashboard\User\Command\Add\",
        "Features\Dashboard\User\Command\BulkDelete\",
        "Features\Dashboard\User\Command\Delete\",
        "Features\Dashboard\User\Command\Import\",
        "Features\Dashboard\User\Command\Update\",
        "Features\Dashboard\User\Command\UpdateStatus\",
        "Features\Dashboard\User\Query\GetAll\",
        "Features\Dashboard\User\Query\GetDetails\",
        "Features\Dashboard\User\Query\GetStatistics\",

        "Features\Dashboard\Role\Command\Add\",
        "Features\Dashboard\Role\Command\BulkDelete\",
        "Features\Dashboard\Role\Command\Delete\",
        "Features\Dashboard\Role\Command\Import\",
        "Features\Dashboard\Role\Command\Update\",
        "Features\Dashboard\Role\Command\UpdateStatus\",
        "Features\Dashboard\Role\Query\GetAll\",
        "Features\Dashboard\Role\Query\GetDetails\",
        "Features\Dashboard\Role\Query\GetStatistics\",

        "Features\Dashboard\Category\Command\Add\",
        "Features\Dashboard\Category\Command\BulkDelete\",
        "Features\Dashboard\Category\Command\Delete\",
        "Features\Dashboard\Category\Command\Import\",
        "Features\Dashboard\Category\Command\Update\",
        "Features\Dashboard\Category\Command\UpdateStatus\",
        "Features\Dashboard\Category\Query\GetAll\",
        "Features\Dashboard\Category\Query\GetDetails\",
        "Features\Dashboard\Category\Query\GetStatistics\",

        "Features\Dashboard\Expense\Command\Add\",
        "Features\Dashboard\Expense\Command\BulkDelete\",
        "Features\Dashboard\Expense\Command\Delete\",
        "Features\Dashboard\Expense\Command\Import\",
        "Features\Dashboard\Expense\Command\Update\",
        "Features\Dashboard\Expense\Command\UpdateStatus\",
        "Features\Dashboard\Expense\Query\GetAll\",
        "Features\Dashboard\Expense\Query\GetDetails\",
        "Features\Dashboard\Expense\Query\GetStatistics\",

        "Features\Dashboard\Budget\Command\Add\",
        "Features\Dashboard\Budget\Command\BulkDelete\",
        "Features\Dashboard\Budget\Command\Delete\",
        "Features\Dashboard\Budget\Command\Import\",
        "Features\Dashboard\Budget\Command\Update\",
        "Features\Dashboard\Budget\Command\UpdateStatus\",
        "Features\Dashboard\Budget\Query\GetAll\",
        "Features\Dashboard\Budget\Query\GetDetails\",
        "Features\Dashboard\Budget\Query\GetStatistics\",

        "Features\Dashboard\Notification\Command\Add\",
        "Features\Dashboard\Notification\Command\Delete\",
        "Features\Dashboard\Notification\Command\UpdateStatus\",
        "Features\Dashboard\Notification\Query\GetAll\",
        "Features\Dashboard\Notification\Query\GetDetails\",

        "Features\Dashboard\Setting\Command\Update\",
        "Features\Dashboard\Setting\Query\GetAll\",

        "Features\Dashboard\Statistics\Overview\",
        "Features\Dashboard\Statistics\Monthly\",
        "Features\Dashboard\Statistics\Category\",

        "Features\Reports\Expense\",
        "Features\Reports\Budget\",
        "Features\Reports\Monthly\",
        "Features\Reports\Category\",

        "Services\Email\",
        "Services\Excel\",
        "Services\File\",
        "Services\Helper\",
        "Services\Token\"
    )

    "$ProjectName.Domain" = @(
        "Base\Dto\",
        "Base\Language\",
        "Models\Base\",
        "Models\User\",
        "Models\Expense\",
        "Models\Category\",
        "Models\Budget\",
        "Models\Notification\",
        "Models\AuditLog\",
        "Shared\Attribute\",
        "Shared\Enum\",
        "Shared\Resources\"
    )

    "$ProjectName.Infrastructure" = @(
        "Data\",
        "Migrations\",
        "Seeder\"
    )
}

function Add-FolderItemsToProject {
    param(
        [string]$ProjectFolder,
        [string[]]$Folders
    )

    $projectDir = Join-Path $Root $ProjectFolder
    $csproj = Join-Path $projectDir "$ProjectFolder.csproj"

    if (-not (Test-Path $csproj)) {
        throw "Could not find: $csproj"
    }

    # Ensure the physical directories exist.
    foreach ($folder in $Folders) {
        $physical = Join-Path $projectDir ($folder.TrimEnd('\'))
        if (-not (Test-Path $physical)) {
            New-Item -ItemType Directory -Path $physical -Force | Out-Null
        }
    }

    $content = Get-Content -LiteralPath $csproj -Raw

    # Remove only the block previously generated by THIS script, if present.
    $pattern = '(?s)\s*<ItemGroup Label="ZeeleadStyleFolders">.*?</ItemGroup>'
    $content = [regex]::Replace($content, $pattern, '')

    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add('  <ItemGroup Label="ZeeleadStyleFolders">')

    foreach ($folder in ($Folders | Sort-Object -Unique)) {
        $escaped = [System.Security.SecurityElement]::Escape($folder)
        $lines.Add("    <Folder Include=`"$escaped`" />")
    }

    $lines.Add('  </ItemGroup>')
    $block = [Environment]::NewLine + ($lines -join [Environment]::NewLine) + [Environment]::NewLine

    if ($content -notmatch '</Project>') {
        throw "Invalid .csproj file: $csproj"
    }

    $content = $content -replace '</Project>', ($block + '</Project>')
    Set-Content -LiteralPath $csproj -Value $content -Encoding UTF8

    Write-Host "[OK] $ProjectFolder" -ForegroundColor Green
}

Write-Host ""
Write-Host "Restoring empty folders in Solution Explorer without .gitkeep files..." -ForegroundColor Cyan
Write-Host "This script only adds <Folder Include=...> entries to the .csproj files." -ForegroundColor DarkGray
Write-Host ""

foreach ($entry in $projects.GetEnumerator()) {
    Add-FolderItemsToProject -ProjectFolder $entry.Key -Folders $entry.Value
}

Write-Host ""
Write-Host "Done." -ForegroundColor Green
Write-Host "In Visual Studio: reload the projects/solution, or close and reopen ExpenseFlow.sln." -ForegroundColor Yellow
Write-Host "No source .cs file was deleted or overwritten." -ForegroundColor DarkGray
